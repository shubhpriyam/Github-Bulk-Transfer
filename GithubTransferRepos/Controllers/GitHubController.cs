using GithubTransferRepos.Models;
using Microsoft.AspNetCore.Mvc;
using Octokit;

namespace GithubTransferRepos.Controllers
{
    public class GitHubController : Controller
    {
        private readonly GitHubClient _gitHubClient;
        private const string sourceRepo = "";
        private const string destinationRepo = "";
        private const string token = "";

        public GitHubController()
        {
            // Initialize GitHubClient with your Personal Access Token (replace "your-token" with your actual token)
            _gitHubClient = new GitHubClient(new ProductHeaderValue(sourceRepo));
            _gitHubClient.Credentials = new Credentials(token);
        }

        public async Task<ActionResult> Index()
        {
            // Retrieve repositories from authenticated user
            var repositories = await _gitHubClient.Repository.GetAllForCurrent();

            var owners = repositories.GroupBy(x => x.Owner.Login)
                             .Select(group => new OwnerRepositoryModel
                             {
                                 OwnerName = group.Key,
                                 OwnerAvtarUrl = group.First().Owner.AvatarUrl, // Fetch the AvatarUrl from the first repository in the group
                                 Repositories = group.Select(repo => new GitHubRepositoryModel
                                 {
                                     Id = repo.Id,
                                     Name = repo.Name,
                                     IsSelected = false,
                                 }).ToList()
                             }).ToList();

            //var viewModel = new GitHubSourceDestinationViewModel
            //{
            //    OwnerRepositoriesList = owners,
            //    DestinationOwners = repositories.GroupBy(x => x.Owner.Login).Select(x => x.Key).ToList()
            //};

            var list = repositories.Select(repo => new GitHubRepositoryModel
            {
                Name = repo.Name,
                IsSelected = false
            }).ToList();

            return View(list);
        }

        [HttpPost]
        public async Task<ActionResult> Transfer(List<GitHubRepositoryModel> repositories)
        {
            await TransferRepositories(repositories);
            ViewBag.success = true;
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task TransferRepositories(List<GitHubRepositoryModel> repositories)
        {
            try
            {
                var selectedRepos = repositories.Where(repo => repo.IsSelected).Select(repo => repo.Id).ToList();

                // Transfer each selected repository to organization
                foreach (var id in selectedRepos)
                {
                    await _gitHubClient.Repository.Transfer(id, new RepositoryTransfer(destinationRepo));
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }
        }
    }

}
