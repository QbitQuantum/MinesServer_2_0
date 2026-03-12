using MinesServer.GameShit.Buildings;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.GUI.Horb.List;
using MinesServer.GameShit.SysCraft;
using MinesServer.GameShit.SysMarket;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Network.GUI;
using MinesServer.Server;

namespace MinesServer.GameShit.Sys_Craft
{
    public static class StaticSystem
    {
        private static readonly string[] crysNames =
        {
            "<color=#00e600>Зелёный кристалл</color>",
            "<color=#2929ff>Синий кристалл</color>",
            "<color=#ff3333>Красный кристалл</color>",
            "<color=purple>Фиолетовый кристалл</color>",
            "<color=white>Белый кристалл</color>",
            "<color=cyan>Голубой кристалл</color>"
        };

        private static InventoryItem[] Items()
        {
            var items = new List<InventoryItem>();

            // Show all known market items so the player can see the full catalog.
            for (int i = 0; i <= 50; i++)
            {
                if (i == 8) continue;  // Cyan Alive
                if (i == 11) continue;  // Cyan Alive
                if (i == 12) continue;  // Red Alive
                if (i == 13) continue;  // Violet Alive
                if (i == 14) continue;  // Black Alive
                if (i == 15) continue;  // White Alive
                if (i == 16) continue;  // Blue Alive
                if (i == 31) continue;  // X3
                if (i == 32) continue;  // FreeUP
                if (i == 33) continue;  // MineX4
                if (i == 34) continue;  // Gypno Alive
                if (i == 46) continue;  // Rainbow Alive
                if (i == 49) continue;  // Деньги
                if (i == 50) continue;  // ОПП

                string UpText = "0"; // Stub, should depend on how much quantity is available depending on the player's resources
                string DownText = "<color=#ff3333>+++</color>"; // Stub, should depend on the level of resource pumping(+/++/+++)
                items.Add(InventoryItem.Item(
                    i,
                    upText: UpText,
                    downText: DownText,
                    faint: false,
                    upTextColor: InventoryTextColor.Default,
                    downTextColor: InventoryTextColor.Green));
            }

            return items.ToArray();
        }

        private static string BuildRequirementsText(Recipie recipe)
        {
            var lines = new List<string>();

            if (recipe.costcrys is { Length: > 0 })
            {
                foreach (var cry in recipe.costcrys)
                {
                    var crystalName = cry.id >= 0 && cry.id < crysNames.Length
                        ? crysNames[cry.id]
                        : $"Кристалл #{cry.id}";
                    lines.Add($"{crystalName} x{cry.num}");
                }
            }

            if (recipe.costres is { Length: > 0 })
            {
                foreach (var res in recipe.costres)
                {
                    lines.Add($"{MarketSystem.PackName(res.id)} x{res.num}");
                }
            }

            if (lines.Count == 0)
            {
                lines.Add("Без требований");
            }

            return string.Join("\n", lines);
        }

        private static void OpenRecipie(Player p, int id)
        {
            var recipe = RDes.recipies.FirstOrDefault(i => i.id == id);
            if (recipe.result.id == 0 && recipe.time == 0)
            {
                return;
            }

            var requirementsText = BuildRequirementsText(recipe);
            var itemName = MarketSystem.PackName(recipe.result.id);

            p.win?.CurrentTab.Open(new Page
            {
                Title = $"Рецепт: {itemName}",
                Card = new Card(
                    CardImageType.Item,
                    recipe.result.id.ToString(),
                    $"{itemName} x{recipe.result.num}\nВремя сборки: {recipe.time} сек."),
                Text = $"@@\nНеобходимо для сборки:\n\n{requirementsText}\n\n",
                Input = new InputConfig("Количество", null, false),
                Buttons =
                [
                    new MButton(
                        "Собрать",
                        $"craft:{ActionMacros.Input}",
                        a =>
                        {
                            if (int.TryParse(a.Input, out var num))
                            {
                                Craft(p, recipe, num);
                            }
                        })
                ],
            });
        }

        private static void Craft(Player p, Recipie r, int num)
        {
            if (!World.ContainsPack(p.x, p.y, out var craft) || num <= 0)
            {
                p.connection?.SendU(new OKPacket("Невозможно начать крафт", "Подойдите к крафтеру и попробуйте снова."));
                return;
            }

            if (!MeetReqs(p, r, num))
            {
                p.connection?.SendU(new OKPacket("Недостаточно ресурсов", "У вас не хватает ресурсов или кристаллов для этого крафта."));
                return;
            }

            DeleteReqs(p, r, num);

            var c = craft as Crafter;
            if (c is null)
            {
                p.connection?.SendU(new OKPacket("Ошибка крафта", "Здание крафтера не найдено."));
                return;
            }

            using var db = new DataBase();
            db.crafts.Attach(c);

            c.currentcraft = new CraftEntry(r.id, num, DateTime.Now + (TimeSpan.FromSeconds(r.time) * num));
            c.ready = false;

            db.SaveChanges();
            p.win?.CurrentTab.Open(FilledPage(p, c));
            World.W.GetChunk(c.x, c.y).ResendPack(c);
            p.SendInventory();
        }

        private static void Claim(Player p, Crafter c)
        {
            if (c.currentcraft is null)
            {
                return;
            }

            var recipe = c.currentcraft.GetRecipie();

            using var db = new DataBase();
            db.crafts.Attach(c);
            db.players.Attach(p);

            p.inventory[recipe.result.id] += c.currentcraft.num * recipe.result.num;
            db.craftentries.Remove(c.currentcraft);
            c.currentcraft = null;
            c.ready = false;

            db.SaveChanges();

            p.SendInventory();
            World.W.GetChunk(c.x, c.y).ResendPack(c);
            p.win = c.GUIWin(p);
        }

        private static bool MeetReqs(Player p, Recipie r, int num) =>
            (r.costcrys is null || !r.costcrys.Select(i => p.crys.cry[i.id] >= (i.num * num)).Contains(false)) &&
            (r.costres is null || !r.costres.Select(i => p.inventory[i.id] >= (i.num * num)).Contains(false));

        private static void DeleteReqs(Player p, Recipie r, int num)
        {
            if (r.costcrys is not null)
            {
                foreach (var i in r.costcrys)
                {
                    p.crys.RemoveCrys(i.id, i.num * num);
                }
            }

            if (r.costres is not null)
            {
                foreach (var i in r.costres)
                {
                    p.inventory[i.id] -= i.num * num;
                }
            }

            p.SendInventory();
        }
        public static IPage? FilledPage(Player p, Crafter c)
        {
            if (c.currentcraft is null)
            {
                return GlobalFirstPage(p);
            }

            var rawProgress = c.currentcraft.progress;
            var clamped = rawProgress <= 100 ? rawProgress : 100;
            var progress = (int)Math.Round(clamped);

            var filled = (int)(progress / 2);
            var empty = 50 - filled;
            var bar = "<color=#aaeeaa>" + new string('|', filled) + "</color>" + new string('-', empty);

            var remainingTimeSpan = c.currentcraft.endtime - DateTime.Now;
            var remaining = progress != 100
                ? $"Осталось времени: {remainingTimeSpan:hh\\:mm\\:ss}"
                : "Крафт завершён. Заберите результат.";

            var text = $"@@\nПрогресс: {progress}% {bar}\n\n{remaining}";

            // Создаем список кнопок
            var buttons = new List<MButton>();

            // Всегда добавляем кнопку для забора готовых предметов (если есть хоть один готовый)
            if (c.currentcraft.num > 0)
            {
                // Проверяем, есть ли уже готовые предметы
                var readyItems = 0;
                var recipe = c.currentcraft.GetRecipie();
                // Рассчитываем количество готовых предметов на основе прошедшего времени
                var elapsed = DateTime.Now - (c.currentcraft.endtime - TimeSpan.FromSeconds(recipe.time * c.currentcraft.num));
                var totalSeconds = recipe.time * c.currentcraft.num;
                var progressSeconds = Math.Min(elapsed.TotalSeconds, totalSeconds);
                readyItems = (int)(progressSeconds / recipe.time);

                if (readyItems > 0)
                {
                    buttons.Add(new MButton(
                        $"Забрать готовые ({readyItems} шт.)",
                        "claimready",
                        _ => ClaimReady(p, c, readyItems)));
                }
            }

            // Добавляем кнопку завершения, если крафт полностью готов
            if (c.currentcraft.progress >= 100)
            {
                buttons.Add(new MButton("Забрать всё", "claimall", _ => Claim(p, c)));
            }

            return new Page
            {
                Title = "Крафтер",
                Text = text,
                Buttons = buttons.ToArray(),
            };
        }

        private static void ClaimReady(Player p, Crafter c, int readyCount)
        {
            if (c.currentcraft is null || readyCount <= 0)
            {
                return;
            }

            var recipe = c.currentcraft.GetRecipie();

            using var db = new DataBase();
            db.crafts.Attach(c);
            db.players.Attach(p);

            // Добавляем готовые предметы в инвентарь
            p.inventory[recipe.result.id] += readyCount * recipe.result.num;

            // Уменьшаем количество в текущем крафте
            c.currentcraft.num -= readyCount;

            // Если все предметы забрали, удаляем крафт
            if (c.currentcraft.num <= 0)
            {
                db.craftentries.Remove(c.currentcraft);
                c.currentcraft = null;
                c.ready = false;
            }
            else
            {
                // Корректируем время окончания для оставшихся предметов
                var remainingTime = TimeSpan.FromSeconds(recipe.time * c.currentcraft.num);
                c.currentcraft.endtime = DateTime.Now + remainingTime;
            }

            db.SaveChanges();

            p.SendInventory();
            World.W.GetChunk(c.x, c.y).ResendPack(c);

            // Обновляем интерфейс
            if (c.currentcraft != null)
            {
                p.win?.CurrentTab.Open(FilledPage(p, c));
            }
            else
            {
                p.win = c.GUIWin(p);
            }
        }
        public static IPage? GlobalFirstPage(Player p)
        {
            var onInventoryClick = new Action<int>(type =>
            {
                var recipesForItem = RDes.recipies.Where(r => r.result.id == type).ToArray();

                if (recipesForItem.Length == 0)
                {
                    p.win?.CurrentTab.Open(new Page
                    {
                        Title = "Крафтер",
                        Text = "@@\nДля этого предмета пока нет рецептов.\n",
                        Buttons =
                        [
                            new MButton("Назад", "back", _ => p.win?.CurrentTab.Open(GlobalFirstPage(p)))
                        ],
                    });
                    return;
                }

                // Open the first available recipe immediately (no intermediate list).
                OpenRecipie(p, recipesForItem[0].id);
            });

            return new Page
            {
                Title = "Крафтер",
                OnInventory = onInventoryClick,
                Inventory = Items(),
                Buttons = [],
                Style = new Style
                {
                    Inventory = new GridStyle
                    {
                        CellHeight = 65,
                    },
                }
            };
        }
    }
}