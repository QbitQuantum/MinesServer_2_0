using Microsoft.EntityFrameworkCore;
using MinesServer.GameShit.ClanSystem;

namespace MinesServer.GameShit.ClanSystem
{
    internal static class ClanQueryExtensions
    {
        public static IQueryable<Clan> WithGraph(this IQueryable<Clan> q) => q
            .Include(c => c.members).ThenInclude(m => m.clanrank)
            .Include(c => c.ranks)
            .Include(c => c.reqs).ThenInclude(r => r.player);

        public static IQueryable<Clan> WithGraphLight(this IQueryable<Clan> q) => q
            .Include(c => c.members)
            .Include(c => c.reqs);
    }
}
