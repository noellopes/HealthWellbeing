
namespace HealthWellbeing.Models
{
	public class Repository
	{
		private static List<Client> clients = new List<Client>();

		public static IEnumerable<Client> ClientList => clients;

		public static void AddClient(Client client) => clients.Add(client);

		public static IEnumerable<Client> GetAllClients() => clients;
	}
}
