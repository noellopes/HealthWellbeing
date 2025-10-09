namespace HealthWellbeing.Models
{
	public class Repository
	{
		private static List<Client> clients = new List<Client>();

		public static IEnumerable<Client> ClientList => clients;

		public static void AddClient(Client client) => clients.Add(client);

		// Members

        private static List<Member> members = new List<Member>();

        public static IEnumerable<Member> MemberList => members;

        public static void AddMember(Member member) => members.Add(member);
    }
}
