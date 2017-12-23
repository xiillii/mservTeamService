using System.Collections;
using System.Collections.Generic;
using mserv.TeamService.Models;
using mserv.TeamService.Persistence;

namespace mserv.TeamService.Tests
{
    public class TestMemoryTeamRepository : MemoryTeamRepository
    {
        public TestMemoryTeamRepository() : base(CreateInitialFake())
        {

        }

        private static ICollection<Team> CreateInitialFake()
        {
            var teams = new List<Team> {new Team("one"), new Team("two")};

            return teams;
        }
    }
}