using System.Security.Cryptography;
using System.Text;
using MinesServer.GameShit.Entities.PlayerStaff;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.Horb;
using MinesServer.GameShit.WorldSystem;
using MinesServer.Network.Auth;
using MinesServer.Network.BotInfo;
using MinesServer.Network.World;


namespace MinesServer.Server
{
    public class Auth
    {
        private readonly Session _initiator;
        private int _pendingId = -1;
        private string _pendingNickname = "";
        private string _pendingPassword = "";
        private Window AuthWindow { get; set; }

        public Auth(Session initiator)
        {
            _initiator = initiator ?? throw new ArgumentNullException(nameof(initiator));
            AuthWindow = CreateDefaultWindow();
        }

        public void ProcessAction(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            if (text.StartsWith("exit", StringComparison.OrdinalIgnoreCase))
            {
                ResetAuthState();
                SendCurrentWindow();
                return;
            }

            AuthWindow?.ProcessButton(text);
            SendCurrentWindow();
        }

        public void TryToAuthenticate(AUPacket packet, string sessionId)
        {
            Console.WriteLine("Attempting authentication...");

            Player? existingPlayer = null;
            if (packet.user_id.HasValue)
            {
                existingPlayer = DataBase.GetPlayer(packet.user_id.Value);
            }

            SendWorldInfo();

            if (existingPlayer == null)
            {
                HandleNewPlayer();
                return;
            }

            if (existingPlayer != null && CalculateMD5Hash(existingPlayer.hash + sessionId) == packet.token) 
            {
                HandleSpecialPlayer(existingPlayer);
                return;
            }
        }

        private void HandleNewPlayer()
        {
            _initiator.SendU(new BotInfoPacket("Default", 0, 0, -1));
            _initiator.SendU(new HBPacket([World.W.MapPacket(0, 0)]));
            AuthWindow = CreateDefaultWindow();
            SendCurrentWindow();
        }

        private void HandleSpecialPlayer(Player player)
        {
            // Создаем соединение
            _initiator.CreateSession(player);
        }

        public void CreateNewAccount()
        {
            var nicknamePage = new Page
            {
                Title = "НОВЫЙ ИГРОК",
                Text = "Ник",
                Input = new InputConfig
                {
                    IsConsole = true,
                    Placeholder = "Введите никнейм"
                },
                Buttons = [new MButton("OK", $"newnick:{ActionMacros.Input}", OnNicknameEntered)]
            };

            OpenPage(nicknamePage);
        }

        private void OnNicknameEntered(ActionArgs args)
        {
            var nickname = args.Input;

            if (string.IsNullOrWhiteSpace(nickname))
            {
                ShowError("Никнейм не может быть пустым");
                CreateNewAccount();
                return;
            }

            if (DataBase.PlayerExists(nickname))
            {
                ShowError("Ник занят");
                CreateNewAccount();
                return;
            }

            _pendingNickname = nickname;
            ShowPasswordPageForNewAccount();
        }

        private void ShowPasswordPageForNewAccount()
        {
            var passwordPage = new Page
            {
                Title = "НОВЫЙ ИГРОК",
                Text = "Пароль",
                Input = new InputConfig
                {
                    IsConsole = true,
                    Placeholder = "Введите пароль",
                },
                Buttons = [new MButton("OK", $"passwd:{ActionMacros.Input}", OnPasswordEnteredForNewAccount)]
            };

            OpenPage(passwordPage);
        }

        private void OnPasswordEnteredForNewAccount(ActionArgs args)
        {
            var password = args.Input;

            if (string.IsNullOrWhiteSpace(password))
            {
                ShowError("Пароль не может быть пустым");
                ShowPasswordPageForNewAccount();
                return;
            }

            CreatePlayerAccount(_pendingNickname, password);
        }

        private void CreatePlayerAccount(string nickname, string password)
        {
            using var db = new DataBase();

            var _tempPlayer = new Player();

            _tempPlayer.CreatePlayer();
            _tempPlayer.id = DataBase.GetNextId();
            _tempPlayer.name = nickname;
            _tempPlayer.passwd = password;
            _tempPlayer.hash = GenerateHash();

            db.players.Add(_tempPlayer);
            db.skills.Attach(_tempPlayer.skillslist);
            db.SaveChanges();

            _tempPlayer = null;

            CompleteLoginAfterCreation(nickname);
        }

        private void CompleteLoginAfterCreation(string nickname)
        {
            // Получаем данные игрока из БД
            var player = DataBase.GetPlayer(nickname);

            if (player == null)
            {
                ShowError("Ошибка создания аккаунта");
                ResetAuthState();
                return;
            }

            // Создаем соединение
            _initiator.CreateSession(player);
            Console.WriteLine($"Account created and logged in: {player.name}");
        }

        private void TryToFindByNickname(string nickname)
        {
            if (string.IsNullOrWhiteSpace(nickname))
            {
                ShowError("Введите никнейм");
                SendCurrentWindow();
                return;
            }

            var player = DataBase.GetPlayer(nickname);

            if (player == null)
            {
                ShowError("Игрок не найден");
                SendCurrentWindow();
                return;
            }
            _pendingId = player.id;
            _pendingPassword = player.passwd;
            ShowPasswordPageForExistingAccount();
        }

        private void ShowPasswordPageForExistingAccount()
        {
            var passwordPage = new Page
            {
                Text = "Пароль",
                Input = new InputConfig
                {
                    IsConsole = true,
                    Placeholder = "Введите пароль",
                },
                Buttons = [new MButton("OK", $"passwd:{ActionMacros.Input}", OnPasswordEnteredForExistingAccount)]
            };

            OpenPage(passwordPage);
        }

        private void OnPasswordEnteredForExistingAccount(ActionArgs args)
        {
            var password = args.Input;

            if (string.IsNullOrWhiteSpace(password))
            {
                ShowError("Введите пароль");
                ShowPasswordPageForExistingAccount();
                return;
            }

            ValidatePassword(password);
        }

        private void ValidatePassword(string password)
        {
            if (_pendingPassword == password)
            {
                CompleteLogin();
                return;
            }

            ShowError("Не верный пароль");
            ShowPasswordPageForExistingAccount();
        }

        private void CompleteLogin()
        {
            // Получаем данные игрока
            var player = DataBase.GetPlayer(_pendingId);

            if (player == null)
            {
                ShowError("Ошибка загрузки игрока");
                ResetAuthState();
                return;
            }

            // Создаем соединение
            _initiator.CreateSession(player);
            Console.WriteLine($"Player logged in: {player.name}");
        }

        private void ShowError(string message)
        {
            var errorPage = new Page
            {
                Text = $"Ошибка\n{message}",
                Input = new InputConfig
                {
                    IsConsole = true,
                    Placeholder = " "
                },
                Buttons = [new MButton("OK", ActionMacros.Input, args =>
                {
                    // После показа ошибки возвращаемся к начальному окну
                    if (message.Contains("пароль") || message.Contains("Ник"))
                    {
                        AuthWindow = CreateDefaultWindow();
                        SendCurrentWindow();
                    }
                })]
            };

            OpenPage(errorPage);
        }

        private Window CreateDefaultWindow()
        {
            return new Window
            {
                Title = "ВХОД",
                Tabs = [new Tab
                {
                    Label = "Ник",
                    Action = "auth",
                    InitialPage = new Page
                    {
                        Text = "Авторизация",
                        Buttons = [
                            new MButton("Новый аккаунт", "newakk", _ => CreateNewAccount()),
                            new MButton("Авторизация", $"nick:{ActionMacros.Input}", args => TryToFindByNickname(args.Input!))
                        ],
                        Input = new InputConfig
                        {
                            IsConsole = true,
                            Placeholder = "Введите никнейм"
                        }
                    }
                }],
                ShowTabs = false
            };
        }

        private void OpenPage(Page page)
        {
            AuthWindow.CurrentTab.Open(page);
            SendCurrentWindow();
        }

        private void SendCurrentWindow()
        {
            _initiator.SendWin(AuthWindow.ToString());
        }

        private void SendWorldInfo()
        {
            _initiator.SendWorldInfo();
        }

        private void ResetAuthState()
        {
            _pendingId = -1;
            _pendingNickname = "";
            _pendingPassword = "";
            AuthWindow = CreateDefaultWindow();
        }

        private static string CalculateMD5Hash(string input)
        {
            var bytes = Encoding.ASCII.GetBytes(input);
            var hash = MD5.HashData(bytes);
            return Convert.ToHexString(hash).ToLower();
        }

        private static string GenerateHash()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}