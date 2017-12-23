
using System;
using System.Collections.Generic;
using System.Linq;
using mserv.TeamService.Controllers;
using mserv.TeamService.Models;
using mserv.TeamService.Persistence;
using Microsoft.AspNetCore.Mvc;
using Xunit;

[assembly:CollectionBehavior(MaxParallelThreads = 1)]

namespace mserv.TeamService.Tests
{
    public class MembersControllerTest
    {
        [Fact]
        public void TestCreateMemberAddsTeamToList()
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            var controller = new MembersController(repository);

            var teamId = Guid.NewGuid();
            var team = new Team("TestController", teamId);
            repository.Add(team);
            
            var newMemberID = Guid.NewGuid();
            var newMember = new Member(newMemberID);
            controller.CreateMember(newMember, teamId);

            team = repository.Get(teamId);
            
            Assert.True(team.Members.Contains(newMember));
        }

        [Fact]
        public void TestCreateMemberNonexistantTeamReturnsNotFound()
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            var controller = new MembersController(repository);
            
            var teamId = Guid.NewGuid();

            var newMemberId = Guid.NewGuid();
            var newMember = new Member(newMemberId);
            var result = controller.CreateMember(newMember, teamId);
            
            Assert.True(result is NotFoundResult);
        }

        [Fact]
        public void TestGetExistingMemberReturnsMember()
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            var controller = new MembersController(repository);

            var teamId = Guid.NewGuid();
            var team = new Team("TestTeam", teamId);
            var debugTeam = repository.Add(team);

            var memberId = Guid.NewGuid();
            var newMember = new Member(memberId)
            {
                FirstName = "Jim",
                LastName = "Smith"
            };
            controller.CreateMember(newMember, teamId);

            var member = (Member) (controller.GetMember(teamId, memberId) as ObjectResult)?.Value;
            Assert.Equal(member.ID, newMember.ID);
        }

        [Fact]
        public void TestGetMembersReturnsMembers()
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            var controller = new MembersController(repository);

            var teamId = Guid.NewGuid();
            var team = new Team("TestTeam", teamId);
            var debugTeam = repository.Add(team);
            
            var firstMemberId = Guid.NewGuid();
            var newMember = new Member(firstMemberId)
            {
                FirstName = "Jim",
                LastName = "Smith"
            };
            controller.CreateMember(newMember, teamId);

            var secondMemberId = Guid.NewGuid();
            newMember = new Member(secondMemberId)
            {
                FirstName = "John",
                LastName = "Doe"
            };
            controller.CreateMember(newMember, teamId);

            var members = (ICollection<Member>) (controller.GetMembers(teamId) as ObjectResult)?.Value;
            Assert.Equal(2, members.Count);
            Assert.NotNull(members.First(m => m.ID == firstMemberId).ID);
            Assert.NotNull(members.First(m => m.ID == secondMemberId).ID);
        }

        [Fact]
        public void TestGetMembersForNewTeamIsEmpty()
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            var controller = new MembersController(repository);

            var teamId = Guid.NewGuid();
            var team = new Team("TestTeam", teamId);
            var debugTeam = repository.Add(team);

            var members = (ICollection<Member>) (controller.GetMembers(teamId) as ObjectResult).Value;
            Assert.Empty(members);
        }

        [Fact]
        public void TestGetMembersForNonExistantTeamReturnNotFound()
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            var controller = new MembersController(repository);

            var result = controller.GetMembers(Guid.NewGuid());
            Assert.True(result is NotFoundResult);
        }

        [Fact]
        public void TestGetNonExistantTeamReturnsNotFound()
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            var controller = new MembersController(repository);

            var result = controller.GetMember(Guid.NewGuid(), Guid.NewGuid());
            Assert.True(result is NotFoundResult);
        }

        [Fact]
        public void TestGetNonExistantMemberResturnsNotFound()
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            var controller = new MembersController(repository);

            var teamId = Guid.NewGuid();
            var team = new Team("TestTeam", teamId);
            var debugTeam = repository.Add(team);

            var result = controller.GetMember(teamId, Guid.NewGuid());
            Assert.True(result is NotFoundResult);
        }

        [Fact]
        public void TestUpdateMemberOverwrites()
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            var controller = new MembersController(repository);

            var teamId = Guid.NewGuid();
            var team = new Team("TestTeam", teamId);
            var debugTeam = repository.Add(team);

            var memberId = Guid.NewGuid();
            var newMember = new Member(memberId)
            {
                FirstName = "Jim",
                LastName = "Smith"
            };
            controller.CreateMember(newMember, teamId);

            team = repository.Get(teamId);

            var updateMember = new Member(memberId)
            {
                FirstName = "Bob",
                LastName = "Jones"
            };
            controller.UpdateMember(updateMember, teamId, memberId);

            team = repository.Get(teamId);
            var testMember = team.Members.First(m => m.ID == memberId);
            
            Assert.Equal(testMember.FirstName, "Bob");
            Assert.Equal(testMember.LastName, "Jones");
        }

        [Fact]
        public void TestUpdateMemberToNonExistantMemberReturnsNoMatch()
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            var controller = new MembersController(repository);

            var teamId = Guid.NewGuid();
            var team = new Team("TestController", teamId);
            repository.Add(team);

            var memberId = Guid.NewGuid();
            var newMember = new Member(memberId)
            {
                FirstName = "Jim"
            };
            controller.CreateMember(newMember, teamId);

            var nonMatchedGuid = Guid.NewGuid();
            var updateMember = new Member(nonMatchedGuid) {FirstName = "Bob"};
            var result = controller.UpdateMember(updateMember, teamId, nonMatchedGuid);

            Assert.True(result is NotFoundResult);
        }
       
    }
}