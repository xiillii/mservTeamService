using System;
using System.Linq;
using mserv.TeamService.Models;
using mserv.TeamService.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace mserv.TeamService.Controllers
{
    [Route("/teams/{teamId}/[controller]")]
    public class MembersController : Controller
    {
        private readonly ITeamRepository _repository;

        public MembersController(ITeamRepository repo)
        {
            _repository = repo;
        }

        [HttpGet]
        public virtual IActionResult GetMembers(Guid teamId)
        {
            var team = _repository.Get(teamId);

            if (team == null)
            {
                return NotFound();
            }
            return Ok(team.Members);
        }

        [HttpGet]
        [Route("/teams/{teamId}/[controller]/{memberId}")]
        public virtual IActionResult GetMember(Guid teamId, Guid memberId)
        {
            var team = _repository.Get(teamId);

            if (team == null)
            {
                return NotFound();
            }
            var q = team.Members.Where(m => m.ID == memberId);

            if (!q.Any())
            {
                return NotFound();
            }
            return Ok(q.First());
        }

        [HttpPut]
        [Route("/teams/{teamId}/[controller]/{memberId}")]
        public virtual IActionResult UpdateMember([FromBody] Member updateMember, Guid teamId, Guid memberId)
        {
            var team = _repository.Get(teamId);

            if (team == null)
            {
                return NotFound();
            }
            var q = team.Members.Where(m => m.ID == memberId);
            if (!q.Any())
            {
                return NotFound();
            }
            team.Members.Remove(q.First());
            team.Members.Add(updateMember);

            return Ok();
        }

        [HttpPost]
        public virtual IActionResult CreateMember([FromBody] Member newMember, Guid teamId)
        {
            var team = _repository.Get(teamId);
            
            if (team == null)
            {
                return NotFound();
            }
            team.Members.Add(newMember);
            var teamMember = new {TeamID = team.ID, MemberID = newMember.ID};

            return Created($"/teams/{teamMember.TeamID}/[controller]/{teamMember.MemberID}", teamMember);
        }

        public IActionResult GetTeamForMember(Guid memberId)
        {
            var teamId = GetTeamIdForMember(memberId);

            if (teamId != Guid.Empty)
            {
                return Ok(new
                {
                    TeamID = teamId
                });
            }
            return NotFound();
        }

        private Guid GetTeamIdForMember(Guid memberId)
        {
            var res = _repository.List().FirstOrDefault(t => t.Members.Select(m => m.ID == memberId).Any());

            return res?.ID ?? Guid.Empty;
        }
    }
}