using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;

namespace GraphWebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TeamApiController: ControllerBase
    {
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly GraphServiceClient _graphServiceClient;
        private readonly IOptions<MicrosoftGraphOptions> _graphOptions;

        static readonly string[] scopeRequiredByApi = new string[] { "access_as_user" };

        public TeamApiController(ITokenAcquisition tokenAcquisition, GraphServiceClient graphServiceClient, IOptions<MicrosoftGraphOptions> graphOptions)
        {
            _tokenAcquisition = tokenAcquisition;
            _graphServiceClient = graphServiceClient;
            _graphOptions = graphOptions;
        }


        [HttpGet]
        public async Task<ActionResult> GetTeams()
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

            var userTeams = await _graphServiceClient.Me.JoinedTeams.Request().GetAsync();

            if (userTeams == null)
            {
                return NotFound();
            }

            var teams = userTeams.CurrentPage.Select(t => new { t.Id, t.DisplayName, t.Description });

            return Ok(teams);
        }

        [HttpGet("GetChannels/{teamId}")]
        public async Task<ActionResult> GetChannels(string teamId)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

            var teamChannels = await _graphServiceClient.Teams[teamId].Channels.Request().GetAsync();

            if (teamChannels == null)
            {
                return NotFound();
            }

            var channels = teamChannels.CurrentPage.Select(t => new { t.Id, t.DisplayName, t.Description });

            return Ok(teamChannels);
        }
    }
}
