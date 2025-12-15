using HealthWellbeing.Models;
using Microsoft.AspNetCore.Identity;
using HealthWellbeing.Data;
using Microsoft.EntityFrameworkCore;
using HealthWellBeing.Models;

namespace HealthWellbeing.Data
{
    public class SeedDataG6
    {
        // Método principal chamado no Program.cs
        internal static async Task Populate(HealthWellbeingDbContext? dbContext, UserManager<IdentityUser>? userManager, RoleManager<IdentityRole>? roleManager)
        {
            if (dbContext == null || userManager == null || roleManager == null)
                throw new ArgumentNullException("Contextos nulos");

            // Garante que a base de dados existe
            await dbContext.Database.EnsureCreatedAsync();

            // 1. IDENTITY (Roles e Users)
            await SeedRoles(roleManager);
            await SeedDefaultAdmin(userManager);
            await SeedGroupMembers(userManager);

            // 2. DADOS CLÍNICOS
            await SeedDadosClinicos(dbContext);
        }

        internal static async Task SeedDadosClinicos(HealthWellbeingDbContext db)
        {
            // --------------------------------------------------------
            // 1. TABELAS DE APOIO
            // --------------------------------------------------------

            // Especialidades
            if (!db.Especialidades.Any())
            {
                db.Especialidades.AddRange(
                    new Especialidade { Nome = "Cardiologia" },
                    new Especialidade { Nome = "Radiologia" },
                    new Especialidade { Nome = "Clínica Geral" }
                );
                await db.SaveChangesAsync();
            }

            // Funções
            if (!db.Funcoes.Any())
            {
                db.Funcoes.AddRange(
                    new Funcao { NomeFuncao = "Técnico" },
                    new Funcao { NomeFuncao = "Enfermeiro" }
                );
                await db.SaveChangesAsync();
            }

            // Estados Material
            if (!db.EstadosMaterial.Any())
            {
                db.EstadosMaterial.AddRange(
                    new EstadoMaterial { Nome = "Disponível", Descricao = "Pronto a usar" },
                    new EstadoMaterial { Nome = "Em Uso", Descricao = "Ocupado" }
                );
                await db.SaveChangesAsync();
            }

            // --------------------------------------------------------
            // 2. TABELAS PRINCIPAIS
            // --------------------------------------------------------

            // Salas
            if (!db.SalaDeExame.Any())
            {
                db.SalaDeExame.AddRange(
                    new SalaDeExames { TipoSala = "Gabinete 1", Laboratorio = "Lab Central" },
                    new SalaDeExames { TipoSala = "Sala Raio-X", Laboratorio = "Imagiologia" }
                );
                await db.SaveChangesAsync();
            }

            // Utentes
            if (!db.Utentes.Any())
            {
                db.Utentes.AddRange(
                    new Utente
                    {
                        Nome = "Maria Silva",
                        Nif = "200300400",
                        UtenteSNS = 123456789,
                        NumCC = "123456789ZZ1",
                        Email = "maria@exemplo.com",
                        Numero = "910000001",
                        Estado = "Ativo",
                        Morada = "Rua das Flores, 12",
                        CodigoPostal = "1000-200"
                    },
                    new Utente
                    {
                        Nome = "António Costa",
                        Nif = "210310410",
                        UtenteSNS = 987654321,
                        NumCC = "987654321ZZ9",
                        Email = "antonio@exemplo.com",
                        Numero = "960000002",
                        Estado = "Ativo",
                        Morada = "Av. Liberdade, 50",
                        CodigoPostal = "4000-100"
                    }
                );
                await db.SaveChangesAsync();
            }

            // Médicos
            if (!db.Medicos.Any())
            {
                db.Medicos.AddRange(
                    new Medicos
                    {
                        Nome = "Dr. João Santos",
                        Email = "joao.santos@hospital.com",
                        Telefone = "911222333",
                        Especialidade = "Cardiologia"
                    },
                    new Medicos
                    {
                        Nome = "Dra. Ana Sousa",
                        Email = "ana.sousa@hospital.com",
                        Telefone = "911222444",
                        Especialidade = "Clínica Geral"
                    }
                );
                await db.SaveChangesAsync();
            }

            // --------------------------------------------------------
            // 3. TABELAS DEPENDENTES
            // --------------------------------------------------------

            // Tipos de Exame
            if (!db.ExameTipo.Any())
            {
                var espCardio = await db.Especialidades.FirstOrDefaultAsync(e => e.Nome == "Cardiologia");
                var espGeral = await db.Especialidades.FirstOrDefaultAsync(e => e.Nome == "Clínica Geral");

                if (espCardio != null && espGeral != null)
                {
                    db.ExameTipo.AddRange(
                        new ExameTipo { Nome = "Eletrocardiograma", Descricao = "ECG Simples", EspecialidadeId = espCardio.EspecialidadeId },
                        new ExameTipo { Nome = "Check-up Geral", Descricao = "Análises de rotina", EspecialidadeId = espGeral.EspecialidadeId }
                    );
                    await db.SaveChangesAsync();
                }
            }

            // Profissionais
            if (!db.ProfissionalExecutante.Any())
            {
                var funcEnf = await db.Funcoes.FirstOrDefaultAsync(f => f.NomeFuncao == "Enfermeiro");
                if (funcEnf != null)
                {
                    db.ProfissionalExecutante.Add(new ProfissionalExecutante
                    {
                        Nome = "Enf. Pedro",
                        Email = "pedro@hospital.com",
                        Telefone = "920000000",
                        FuncaoId = funcEnf.FuncaoId
                    });
                    await db.SaveChangesAsync();
                }
            }

            // Materiais
            if (!db.MaterialEquipamentoAssociado.Any())
            {
                var estadoDisp = await db.EstadosMaterial.FirstOrDefaultAsync(e => e.Nome == "Disponível");
                if (estadoDisp != null)
                {
                    db.MaterialEquipamentoAssociado.Add(new MaterialEquipamentoAssociado
                    {
                        NomeEquipamento = "Kit ECG",
                        Quantidade = 10,
                        MaterialStatusId = estadoDisp.MaterialStatusId
                    });
                    await db.SaveChangesAsync();
                }
            }

            // ==============================================================================
            // 4. EXAMES (DADOS FICTÍCIOS - LOTE GRANDE)
            // ==============================================================================
            if (!db.Exames.Any())
            {
                // Buscar entidades para relacionar
                var utente1 = await db.Utentes.OrderBy(u => u.UtenteId).FirstOrDefaultAsync(); // Maria
                var utente2 = await db.Utentes.OrderBy(u => u.UtenteId).LastOrDefaultAsync();  // António

                var medico1 = await db.Medicos.OrderBy(m => m.Id).FirstOrDefaultAsync(); // João
                var medico2 = await db.Medicos.OrderBy(m => m.Id).LastOrDefaultAsync();  // Ana

                var sala = await db.SalaDeExame.FirstOrDefaultAsync();
                var tipoExame = await db.ExameTipo.FirstOrDefaultAsync();
                var prof = await db.ProfissionalExecutante.FirstOrDefaultAsync();
                var material = await db.MaterialEquipamentoAssociado.FirstOrDefaultAsync();

                // Garantir que não são nulos antes de criar
                if (utente1 != null && medico1 != null && sala != null && tipoExame != null && prof != null && material != null)
                {
                    // Usar o segundo utente/medico se existirem, senão usa o primeiro
                    var u1 = utente1;
                    var u2 = utente2 ?? utente1;
                    var m1 = medico1;
                    var m2 = medico2 ?? medico1;

                    var exames = new List<Exame>
                    {
                        // 1. Futuro Próximo
                        new Exame { DataHoraMarcacao = DateTime.Now.AddDays(2).AddHours(3), Estado = "Marcado", Notas = "Paciente referiu claustrofobia.", UtenteId = u1.UtenteId, ExameTipoId = tipoExame.ExameTipoId, MedicoSolicitanteId = m1.Id, ProfissionalExecutanteId = prof.ProfissionalExecutanteId, SalaDeExameId = sala.SalaId, MaterialEquipamentoAssociadoId = material.MaterialEquipamentoAssociadoId },
                        
                        // 2. Passado Recente
                        new Exame { DataHoraMarcacao = DateTime.Now.AddDays(-10).AddHours(1), Estado = "Realizado", Notas = "Tudo normal.", UtenteId = u2.UtenteId, ExameTipoId = tipoExame.ExameTipoId, MedicoSolicitanteId = m2.Id, ProfissionalExecutanteId = prof.ProfissionalExecutanteId, SalaDeExameId = sala.SalaId, MaterialEquipamentoAssociadoId = material.MaterialEquipamentoAssociadoId },
                        
                        // 3. Pendente
                        new Exame { DataHoraMarcacao = DateTime.Now.AddDays(5), Estado = "Pendente", Notas = "Aguarda confirmação.", UtenteId = u1.UtenteId, ExameTipoId = tipoExame.ExameTipoId, MedicoSolicitanteId = m1.Id, ProfissionalExecutanteId = prof.ProfissionalExecutanteId, SalaDeExameId = sala.SalaId, MaterialEquipamentoAssociadoId = material.MaterialEquipamentoAssociadoId },
                        
                        // 4. Cancelado
                        new Exame { DataHoraMarcacao = DateTime.Now.AddDays(-2), Estado = "Cancelado", Notas = "Utente faltou.", UtenteId = u2.UtenteId, ExameTipoId = tipoExame.ExameTipoId, MedicoSolicitanteId = m2.Id, ProfissionalExecutanteId = prof.ProfissionalExecutanteId, SalaDeExameId = sala.SalaId, MaterialEquipamentoAssociadoId = material.MaterialEquipamentoAssociadoId },
                        
                        // 5. Marcado
                        new Exame { DataHoraMarcacao = DateTime.Now.AddDays(15).AddHours(2), Estado = "Marcado", Notas = "Revisão anual.", UtenteId = u1.UtenteId, ExameTipoId = tipoExame.ExameTipoId, MedicoSolicitanteId = m1.Id, ProfissionalExecutanteId = prof.ProfissionalExecutanteId, SalaDeExameId = sala.SalaId, MaterialEquipamentoAssociadoId = material.MaterialEquipamentoAssociadoId },
                        
                        // 6. Realizado
                        new Exame { DataHoraMarcacao = DateTime.Now.AddDays(-30), Estado = "Realizado", Notas = "Resultados enviados.", UtenteId = u2.UtenteId, ExameTipoId = tipoExame.ExameTipoId, MedicoSolicitanteId = m2.Id, ProfissionalExecutanteId = prof.ProfissionalExecutanteId, SalaDeExameId = sala.SalaId, MaterialEquipamentoAssociadoId = material.MaterialEquipamentoAssociadoId },
                        
                        // 7. Marcado
                        new Exame { DataHoraMarcacao = DateTime.Now.AddDays(1).AddHours(5), Estado = "Marcado", Notas = "Urgente.", UtenteId = u1.UtenteId, ExameTipoId = tipoExame.ExameTipoId, MedicoSolicitanteId = m1.Id, ProfissionalExecutanteId = prof.ProfissionalExecutanteId, SalaDeExameId = sala.SalaId, MaterialEquipamentoAssociadoId = material.MaterialEquipamentoAssociadoId },
                        
                        // 8. Pendente
                        new Exame { DataHoraMarcacao = DateTime.Now.AddDays(20), Estado = "Pendente", Notas = "Verificar seguro.", UtenteId = u2.UtenteId, ExameTipoId = tipoExame.ExameTipoId, MedicoSolicitanteId = m2.Id, ProfissionalExecutanteId = prof.ProfissionalExecutanteId, SalaDeExameId = sala.SalaId, MaterialEquipamentoAssociadoId = material.MaterialEquipamentoAssociadoId },
                        
                        // 9. Realizado Antigo
                        new Exame { DataHoraMarcacao = DateTime.Now.AddDays(-60), Estado = "Realizado", Notas = "Sem alterações.", UtenteId = u1.UtenteId, ExameTipoId = tipoExame.ExameTipoId, MedicoSolicitanteId = m1.Id, ProfissionalExecutanteId = prof.ProfissionalExecutanteId, SalaDeExameId = sala.SalaId, MaterialEquipamentoAssociadoId = material.MaterialEquipamentoAssociadoId },
                        
                        // 10. Marcado Próximo
                        new Exame { DataHoraMarcacao = DateTime.Now.AddDays(3), Estado = "Marcado", Notas = "Jejum de 8 horas.", UtenteId = u2.UtenteId, ExameTipoId = tipoExame.ExameTipoId, MedicoSolicitanteId = m2.Id, ProfissionalExecutanteId = prof.ProfissionalExecutanteId, SalaDeExameId = sala.SalaId, MaterialEquipamentoAssociadoId = material.MaterialEquipamentoAssociadoId },
                        
                        // 11. Cancelado Recente
                        new Exame { DataHoraMarcacao = DateTime.Now.AddDays(1), Estado = "Cancelado", Notas = "Médico indisponível.", UtenteId = u1.UtenteId, ExameTipoId = tipoExame.ExameTipoId, MedicoSolicitanteId = m1.Id, ProfissionalExecutanteId = prof.ProfissionalExecutanteId, SalaDeExameId = sala.SalaId, MaterialEquipamentoAssociadoId = material.MaterialEquipamentoAssociadoId },
                        
                        // 12. Marcado
                        new Exame { DataHoraMarcacao = DateTime.Now.AddDays(7), Estado = "Marcado", Notas = "Trazer exames anteriores.", UtenteId = u2.UtenteId, ExameTipoId = tipoExame.ExameTipoId, MedicoSolicitanteId = m2.Id, ProfissionalExecutanteId = prof.ProfissionalExecutanteId, SalaDeExameId = sala.SalaId, MaterialEquipamentoAssociadoId = material.MaterialEquipamentoAssociadoId },
                        
                        // 13. Realizado
                        new Exame { DataHoraMarcacao = DateTime.Now.AddDays(-15), Estado = "Realizado", Notas = "Paciente colaborante.", UtenteId = u1.UtenteId, ExameTipoId = tipoExame.ExameTipoId, MedicoSolicitanteId = m1.Id, ProfissionalExecutanteId = prof.ProfissionalExecutanteId, SalaDeExameId = sala.SalaId, MaterialEquipamentoAssociadoId = material.MaterialEquipamentoAssociadoId },
                        
                        // 14. Marcado
                        new Exame { DataHoraMarcacao = DateTime.Now.AddDays(12), Estado = "Marcado", Notas = "Check-up.", UtenteId = u2.UtenteId, ExameTipoId = tipoExame.ExameTipoId, MedicoSolicitanteId = m2.Id, ProfissionalExecutanteId = prof.ProfissionalExecutanteId, SalaDeExameId = sala.SalaId, MaterialEquipamentoAssociadoId = material.MaterialEquipamentoAssociadoId },
                        
                        // 15. Pendente
                        new Exame { DataHoraMarcacao = DateTime.Now.AddDays(4), Estado = "Pendente", Notas = "A confirmar sala.", UtenteId = u1.UtenteId, ExameTipoId = tipoExame.ExameTipoId, MedicoSolicitanteId = m1.Id, ProfissionalExecutanteId = prof.ProfissionalExecutanteId, SalaDeExameId = sala.SalaId, MaterialEquipamentoAssociadoId = material.MaterialEquipamentoAssociadoId },
                        
                        // 16. Marcado Longo Prazo
                        new Exame { DataHoraMarcacao = DateTime.Now.AddDays(30), Estado = "Marcado", Notas = "Pré-operatório.", UtenteId = u2.UtenteId, ExameTipoId = tipoExame.ExameTipoId, MedicoSolicitanteId = m2.Id, ProfissionalExecutanteId = prof.ProfissionalExecutanteId, SalaDeExameId = sala.SalaId, MaterialEquipamentoAssociadoId = material.MaterialEquipamentoAssociadoId },
                        
                        // 17. Realizado Recente
                        new Exame { DataHoraMarcacao = DateTime.Now.AddDays(-3), Estado = "Realizado", Notas = "Ligeira taquicardia.", UtenteId = u1.UtenteId, ExameTipoId = tipoExame.ExameTipoId, MedicoSolicitanteId = m1.Id, ProfissionalExecutanteId = prof.ProfissionalExecutanteId, SalaDeExameId = sala.SalaId, MaterialEquipamentoAssociadoId = material.MaterialEquipamentoAssociadoId }
                    };

                    db.Exames.AddRange(exames);
                    await db.SaveChangesAsync();
                }
            }
        }

        // --- Identity Helpers (Sem alterações) ---
        internal static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Gestor", "Medico", "Utente", "Rececionista", "Tecnico" };
            foreach (var role in roles)
                if (!await roleManager.RoleExistsAsync(role)) await roleManager.CreateAsync(new IdentityRole(role));
        }

        internal static async Task SeedDefaultAdmin(UserManager<IdentityUser> userManager)
        {
            await CriarUtilizadorSeNaoExistir(userManager, "admin@healthwellbeing.com", "Secret123$", "Admin");
        }

        internal static async Task SeedGroupMembers(UserManager<IdentityUser> userManager)
        {
            string[] emails = { "diogomassano@ipg.pt", "dinisgomes@ipg.pt", "rafaelrodrigues@ipg.pt", "joaquimgoncalves@ipg.pt" };
            foreach (var email in emails) await CriarUtilizadorSeNaoExistir(userManager, email, "Secret123$", "Admin");
        }

        private static async Task CriarUtilizadorSeNaoExistir(UserManager<IdentityUser> userManager, string email, string password, string role)
        {
            if (await userManager.FindByNameAsync(email) == null)
            {
                var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded) await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}