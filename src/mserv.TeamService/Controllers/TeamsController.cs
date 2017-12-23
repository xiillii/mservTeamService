using System;
using mserv.TeamService.Models;
using mserv.TeamService.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace mserv.TeamService.Controllers
{
    [Route("[controller]")]
    public class TeamsController : Controller
    {
        private readonly ITeamRepository _repository;

        public TeamsController(ITeamRepository repo)
        {
            _repository = repo;
        }

        [HttpGet]
        public virtual IActionResult GetAllTeams()
        {
            return Ok(_repository.List());
        }

        [HttpGet("{id}")]
        public IActionResult GetTeam(Guid id)
        {
            var team = _repository.Get(id);

            
            if (team != null)
            {
                return Ok(team);
            }
            return NotFound();
        }

        [HttpPost]
        public virtual IActionResult CreateTeam([FromBody] Team newTeam)
        {
            _repository.Add(newTeam);

            return this.Created($"/teams/{newTeam.ID}", newTeam);
        }

        [HttpPut("{id}")]
        public virtual IActionResult UpdateTeam([FromBody] Team team, Guid id)
        {
            team.ID = id;

            if (_repository.Update(team) == null)
            {
                return NotFound();
            }
            return Ok(team);
        }

        [HttpDelete("{id}")]
        public virtual IActionResult DeleteTeam(Guid id)
        {
            var team = _repository.Delete(id);

            if (team == null)
            {
                return NotFound();
            }

            return Ok(team);
        }
    }
}