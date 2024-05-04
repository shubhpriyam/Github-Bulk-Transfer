namespace GithubTransferRepos.Models
{
    public class GitHubRepositoryModel
    {
        public string Name { get; set; }
        public bool IsSelected { get; set; }
        public long Id { get; set; }
    }

    public class GitHubSourceDestinationViewModel
    {
        public List<OwnerRepositoryModel> OwnerRepositoriesList { get; set; }
        public List<string> DestinationOwners { get; set; }
    }

    public class OwnerRepositoryModel
    {
        public string OwnerName { get; set; }
        public string OwnerAvtarUrl { get; set; }
        public List<GitHubRepositoryModel> Repositories { get; set; }
    }

    public class TransferRequestViewModel
    {
        public object OwnerName { get; set; }
        public object Repositories { get; set; }
    }

}
