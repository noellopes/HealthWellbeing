using HealthWellbeing.Models;
using HealthWellbeing.Data;

internal class SeedData
{
	internal static void Populate(HealthWellbeingDbContext? dbContext)
	{
		if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

		dbContext.Database.EnsureCreated();

		//ADICIONAR OS POPULATES AQUI
		PopulateClients(dbContext);
	}

	private static void PopulateClients(HealthWellbeingDbContext dbContext)
	{
		// Check if the Client table already contains data
		if (dbContext.Client.Any()) return;

		dbContext.Client.AddRange(new List<Client>() {
			// Client 1: Fully Populated
			new Client
			{
				ClientId = Guid.NewGuid().ToString("N"), // Use a GUID for a unique ID
				Name = "Alice Wonderland",
				Email = "alice.w@example.com",
				Phone = "555-1234567",
				Address = "10 Downing St, London",
				BirthDate = new DateTime(1990, 5, 15),
				Gender = "Female",
				RegistrationDate = DateTime.Now.AddDays(-30), // Registered a month ago
				CreateMember = true
			},

			// Client 2: Minimal required fields + Pending Member
			new Client
			{
				ClientId = Guid.NewGuid().ToString("N"),
				Name = "Bob The Builder",
				Email = "bob.builder@work.net",
				Phone = "555-9876543",
				Address = "Construction Site 5A",
				BirthDate = new DateTime(1985, 10, 20),
				Gender = "Male",
				RegistrationDate = DateTime.Now.AddDays(-15),
				CreateMember = null // Pending Acceptation
			},

			// Client 3: Member Creation Rejected
			new Client
			{
				ClientId = Guid.NewGuid().ToString("N"),
				Name = "Charlie Brown",
				Email = "charlie.b@peanuts.com",
				Phone = "555-4567890",
				Address = "123 Comic Strip Ave",
				BirthDate = new DateTime(2000, 1, 1),
				Gender = "Male",
				RegistrationDate = DateTime.Now.AddDays(-5),
				CreateMember = false // Rejected/Denied
			}
		});

		dbContext.SaveChanges();
	}
}