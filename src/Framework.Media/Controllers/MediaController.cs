using Borg.Framework.Media.Models;
using Borg.Framework.MVC;
using Borg.Framework.MVC.BuildingBlocks.Devices;
using Borg.Framework.System;
using Borg.Infra.Relational;
using Borg.Infra.Storage.Assets;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Borg.Framework.Media
{
    [Area("backoffice")]
    public class MediaController : BackofficeController
    {
        private readonly IMediaService _mediaService;

        public MediaController(IBackofficeService<BorgSettings> systemService, IMediaService mediaService) : base(systemService)
        {
            _mediaService = mediaService;
        }

        public async Task<IActionResult> Index(string q)
        {
            var page = new PageContent()
            {
                Title = "Media manager"
            };

            Expression<Func<AssetSpec, bool>> where = x => true;
            if (!string.IsNullOrWhiteSpace(q))
            {
                var filter = q.TrimStart().TrimEnd();
                where = x => x.Name.Contains(filter) || x.Versions.Any(v => v.FileSpec.Name.Contains(filter));
                page.Subtitle = $"filter: {filter}";
                var id = -1;
                if (int.TryParse(filter, out id))
                {
                    where = x => x.Id == id;
                    page.Subtitle = $"filter: [Id] {filter}";
                }
            }

            var model = await _mediaService.Assets(where, Pager.Current, Pager.RowCount,
                new[] { new OrderByInfo<AssetSpec>() { Ascending = false, Property = x => x.Id } }, spec => spec.Versions);

            PageContent(page);
            return View(model);
        }

        public async Task<IActionResult> Asset(int id)
        {
            var model = await _mediaService.Get(id);
            PageContent(new PageContent() { Title = model.Name, Subtitle = model.CurrentFile.FileSpec.MimeType });
            return View(model);
        }

        public IActionResult NewFile()
        {
            PageContent(new PageContent() { Title = "New file" });
            return View(new UploadAssetViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> NewFile(UploadAssetViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name)) model.Name = Path.GetFileName(model.File.FileName);
            if (ModelState.IsValid)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await model.File.CopyToAsync(memoryStream);

                    var asset = await _mediaService.Create(model.Name, memoryStream.ToArray(), model.File.FileName, AssetState.Active, model.File.ContentType);
                    return RedirectToAction("Index", new { q = asset.Id });
                }
            }
            return (View(model));
        }

        [HttpPost]
        public async Task<IActionResult> AssetName(AssetNameViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _mediaService.AssetChangeName(model.Id, model.Name);
            }
            return RedirectToAction("Asset", new { id = model.Id });
        }

        [HttpPost]
        public async Task<IActionResult> AssetDelete(int id)
        {
            if (ModelState.IsValid)
            {
                await _mediaService.Delete(id);
            }
            return RedirectToAction("index", null);
        }

        [HttpPost]
        public async Task<IActionResult> NewVersion(NewVersionViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await model.File.CopyToAsync(memoryStream);

                    await _mediaService.AddNewVersion(model.Id, memoryStream.ToArray(), model.File.FileName, model.File.ContentType);
                }
            }
            return RedirectToAction("Asset", new { id = model.Id });
        }
    }
}