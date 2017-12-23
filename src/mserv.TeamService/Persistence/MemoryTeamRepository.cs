using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using mserv.TeamService.Models;

namespace mserv.TeamService.Persistence
{
    public class MemoryTeamRepository : ITeamRepository
    {
        protected static ICollection<Team> Teams;

        public MemoryTeamRepository()
        {
            if (Teams == null)
            {
                Teams = new List<Team>();
            }
        }

        public MemoryTeamRepository(ICollection<Team> teams) => Teams = teams;

        public IEnumerable<Team> List() => Teams;

        public Team Get(Guid id) => Teams.FirstOrDefault(t => t.ID == id);

        public Team Update(Team t)
        {
            var team = this.Delete(t.ID);

            if (team != null)
            {
                team = Add(t);
            }

            return team;
        }

        public Team Add(Team team)
        {
            Teams.Add(team);
            return team;
        }

        public Team Delete(Guid id)
        {
            var q = Teams.Where(t => t.ID == id);
            Team team = null;

            if (q.Any())
            {
                team = q.First();
                Teams.Remove(team);
            }

            return team;
        }
    }
}