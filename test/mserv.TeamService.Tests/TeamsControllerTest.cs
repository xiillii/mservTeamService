using System;
using System.Collections.Generic;
using System.Linq;
using mserv.TeamService.Controllers;
using mserv.TeamService.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace mserv.TeamService.Tests
{
    public class TeamsControllerTest
    {
        [Fact]
        public void TestQueryTeamListReturnsCorrectTeams()
        {
            var controller = new TeamsController(new TestMemoryTeamRepository());
            var rawTeams = (IEnumerable<Team>) (controller.GetAllTeams() as ObjectResult).Value;
            var teams = new List<Team>(rawTeams);
            Assert.Equal(teams.Count, 2);
            Assert.Equal(teams[0].Name, "one");
            Assert.Equal(teams[1].Name, "two");
        }

        [Fact]
        public void TestGetTeamRetrievesTeam()
        {
            var controller = new TeamsController(new TestMemoryTeamRepository());

            var sampleName = "sample";
            var id = Guid.NewGuid();
            var sampleTeam = new Team(sampleName, id);
            controller.CreateTeam(sampleTeam);

            var retrieveTeam = (Team) (controller.GetTeam(id) as ObjectResult).Value;
            Assert.Equal(retrieveTeam.Name, sampleName);
            Assert.Equal(retrieveTeam.ID, id);
        }

        [Fact]
        public void TestGetNonExistentTeamReturnsNotFound()
        {
            var controller = new TeamsController(new TestMemoryTeamRepository());
            var id = Guid.NewGuid();
            var result = controller.GetTeam(id);
            Assert.True(result is NotFoundResult);
        }

        [Fact]
        public void TestCreateTeamAddsTeamToList()
        {
            var controller = new TeamsController(new TestMemoryTeamRepository());
            var teams = (IEnumerable<Team>) (controller.GetAllTeams() as ObjectResult).Value;
            var original = new List<Team>(teams);
            
            var t = new Team("sample");
            var result = controller.CreateTeam(t);
            
            Assert.Equal(201, (result as ObjectResult).StatusCode);

            var actionResult = controller.GetAllTeams() as ObjectResult;

            var newTeamsRaw = (IEnumerable<Team>) (controller.GetAllTeams() as ObjectResult).Value;
            var newTeams = new List<Team>(newTeamsRaw);
            
            Assert.Equal(original.Count + 1, newTeams.Count);

            var sampleTeam = newTeams.FirstOrDefault(target => target.Name == "sample");
            
            Assert.NotNull(sampleTeam);

        }

        [Fact]
        public void TestUpdateTeamModifiesTeamToList()
        {
            var controller = new TeamsController(new TestMemoryTeamRepository());
            var teams = (IEnumerable<Team>) (controller.GetAllTeams() as ObjectResult).Value;
            var original = new List<Team>(teams);

            var id = Guid.NewGuid();
            var t = new Team("sample", id);
            var result = controller.CreateTeam(t);
            
            var newTeam = new Team("sample2", id);
            controller.UpdateTeam(newTeam, id);

            var newTeamsRaw = (IEnumerable<Team>) (controller.GetAllTeams() as ObjectResult).Value;
            var newTeams = new List<Team>(newTeamsRaw);
            var sampleTeam = newTeams.FirstOrDefault(target => target.Name == "sample");
            Assert.Null(sampleTeam);

            var retrievedTeam = (Team) (controller.GetTeam(id) as ObjectResult).Value;
            Assert.Equal(retrievedTeam.Name, "sample2");
        }

        [Fact]
        public void TestUpdateNonExistentTeamReturnsNotFound()
        {
            var controller = new TeamsController(new TestMemoryTeamRepository());
            var teams = (IEnumerable<Team>) (controller.GetAllTeams() as ObjectResult).Value;
            var original = new List<Team>(teams);
            
            var someTeam = new Team("Some Team", Guid.NewGuid());
            controller.CreateTeam(someTeam);

            var newTeamId = Guid.NewGuid();
            var newTeam = new Team("New Team", newTeamId);
            var result = controller.UpdateTeam(newTeam, newTeamId);
            
            Assert.True(result is NotFoundResult);
        }

        [Fact]
        public void TestDeleteTeamRemovesFromList()
        {
            var controller = new TeamsController(new TestMemoryTeamRepository());
            var teams = (IEnumerable<Team>) (controller.GetAllTeams() as ObjectResult).Value;
            var ct = teams.Count();

            var sampleName = "sample";
            var id = Guid.NewGuid();
            var sampleTeam = new Team(sampleName, id);
            controller.CreateTeam(sampleTeam);

            teams = (IEnumerable<Team>) (controller.GetAllTeams() as ObjectResult).Value;
            sampleTeam = teams.FirstOrDefault(target => target.Name == sampleName);
            Assert.NotNull(sampleName);

            controller.DeleteTeam(id);

            teams = (IEnumerable<Team>) (controller.GetAllTeams() as ObjectResult).Value;
            sampleTeam = teams.FirstOrDefault(target => target.Name == sampleName);
            Assert.Null(sampleTeam);
        }

        [Fact]
        public void TestDeleteNonExistentTeamReturnsNotFound()
        {
            var controller = new TeamsController(new TestMemoryTeamRepository());
            var id = Guid.NewGuid();

            var result = controller.DeleteTeam(id);
            Assert.True(result is NotFoundResult);
        }
    }
}