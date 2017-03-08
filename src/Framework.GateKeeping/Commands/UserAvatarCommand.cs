using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Borg.Framework.GateKeeping.Models;
using Borg.Framework.GateKeeping.Models.UserViewModels;
using Borg.Infra.CQRS;
using Borg.Infra.Storage.Assets;
using Microsoft.AspNetCore.Identity;

namespace Borg.Framework.GateKeeping.Commands
{
    public class UserAvatarCommand : ICommand
    {
        public UserAvatarCommand(UserAvatarViewModel model)
        {
            Model = model;
        }

        public UserAvatarViewModel Model { get; }
    }

    public class UserAvatarCommandHandler : IHandlesCommand<UserAvatarCommand>
    {
        private readonly UserManager<BorgUser> _userManager;
        private readonly IAssetService<int> _assetService;
        private readonly IAssetUrlResolver _assetUrlResolver;

        public UserAvatarCommandHandler(UserManager<BorgUser> userManager, IAssetService<int> assetService, IAssetUrlResolver assetUrlResolver)
        {
            _userManager = userManager;
            _assetService = assetService;
            _assetUrlResolver = assetUrlResolver;
        }

        public async Task<ICommandResult> Execute(UserAvatarCommand message)
        {
            var user = await _userManager.FindByIdAsync(message.Model.UserId);
            if (user == null)
                return CommandResult.Create(false, $"Borg User {message.Model.UserName} with Id {message.Model.UserId} does not exist.");
            var useFile = message.Model.File != null && message.Model.File.Length > 1;
            if (useFile)
            {         
                var file = message.Model.File;
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);

                    var asset = await _assetService.Create(file.Name, memoryStream.ToArray(), file.FileName, AssetState.Active, file.ContentType);
                    message.Model.UserAvatarUrl = _assetUrlResolver.ResolveSourceUrlFromFullPath(asset.CurrentFile.FileSpec.FullPath);
                }       
            }
            var claims = await _userManager.GetClaimsAsync(user);
            var oldClaim = claims.FirstOrDefault(x => x.Type == BorgSpecificClaims.Profile.Avatar);
            if (oldClaim != null) await _userManager.RemoveClaimAsync(user, oldClaim);
            await _userManager.AddClaimAsync(user,
                new Claim(BorgSpecificClaims.Profile.Avatar, message.Model.UserAvatarUrl));
            return CommandResult.Create(true);

        }
    }
}