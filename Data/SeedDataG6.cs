using HealthWellbeing.Models;
using HealthWellBeing.Models; // Namespace alternativo caso exista
using Microsoft.AspNetCore.Identity;
using HealthWellbeing.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeing.Data
{
    public class SeedDataG6
    {
        public static async Task Populate(HealthWellbeingDbContext? dbContext, UserManager<IdentityUser>? userManager, RoleManager<IdentityRole>? roleManager)
        {
            if (dbContext == null || userManager == null || roleManager == null)
                throw new ArgumentNullException("Contextos nulos");

            // Garante que a BD existe
            await dbContext.Database.EnsureCreatedAsync();

            // 1. Identity (Users e Roles)
            await SeedRoles(roleManager);
            await SeedDefaultAdmin(userManager);
            await SeedGroupMembers(userManager);
            await SeedProfissionalLogin(userManager);
            await SeedRecepcionistaLogin(userManager);

            // 2. Dados Clínicos
            await SeedDadosClinicos(dbContext);
        }

        internal static async Task SeedDadosClinicos(HealthWellbeingDbContext db)
        {
            var rnd = new Random();

            // ==============================================================================
            // 1. TABELAS DE APOIO (Especialidades, Funções, Estados)
            // ==============================================================================
            // Nota: Mesmo que o Médico agora use 'string Especialidade', a tabela Especialidade 
            // ainda pode ser usada para o dropdown do 'ExameTipo' ou para consistência.

            if (!db.Especialidades.Any())
            {
                db.Especialidades.AddRange(
                    new Especialidade { Nome = "Cardiologia" },
                    new Especialidade { Nome = "Radiologia" },
                    new Especialidade { Nome = "Clínica Geral" },
                    new Especialidade { Nome = "Ortopedia" },
                    new Especialidade { Nome = "Neurologia" }
                );
                await db.SaveChangesAsync();
            }

            if (!db.Funcoes.Any())
            {
                db.Funcoes.Add(new Funcao { NomeFuncao = "Técnico de Saúde" });
                await db.SaveChangesAsync();
            }

            if (!db.EstadosMaterial.Any())
            {
                db.EstadosMaterial.Add(new EstadoMaterial { Nome = "Operacional" });
                await db.SaveChangesAsync();
            }

            // ExameTipo (Precisa existir para criar Exames)
            if (!db.ExameTipo.Any())
            {
                var esp = await db.Especialidades.FirstOrDefaultAsync();
                int espId = esp?.EspecialidadeId ?? 1;

                db.ExameTipo.AddRange(
                    new ExameTipo { Nome = "Análises Sangue", Descricao = "Rotina", EspecialidadeId = espId },
                    new ExameTipo { Nome = "Raio-X Tórax", Descricao = "Imagem", EspecialidadeId = espId },
                    new ExameTipo { Nome = "ECG Repouso", Descricao = "Cardio", EspecialidadeId = espId },
                    new ExameTipo { Nome = "Ressonância Magnética", Descricao = "Imagem Avançada", EspecialidadeId = espId }
                );
                await db.SaveChangesAsync();
            }

            // Profissional e Material (Precisa existir para criar Exames)
            if (!db.ProfissionalExecutante.Any())
            {
                var func = await db.Funcoes.FirstOrDefaultAsync();
                db.ProfissionalExecutante.Add(new ProfissionalExecutante { Nome = "Téc. Rui", Email = "rui@h.pt", Telefone = "123456789", FuncaoId = func.FuncaoId });
                await db.SaveChangesAsync();
            }

            // MaterialEquipamentoAssociado
            if (!db.MaterialEquipamentoAssociado.Any())
            {
                db.MaterialEquipamentoAssociado.AddRange(
                    // --- LUVAS E PROTEÇÃO ---
                    new MaterialEquipamentoAssociado { NomeEquipamento = "Luvas de Látex", Quantidade = 200, Tamanho = "S" },
                    new MaterialEquipamentoAssociado { NomeEquipamento = "Luvas de Látex", Quantidade = 350, Tamanho = "M" },
                    new MaterialEquipamentoAssociado { NomeEquipamento = "Luvas de Látex", Quantidade = 200, Tamanho = "L" },
                    new MaterialEquipamentoAssociado { NomeEquipamento = "Máscara Cirúrgica Descartável", Quantidade = 500, Tamanho = "Único" },
                    new MaterialEquipamentoAssociado { NomeEquipamento = "Bata Cirúrgica Estéril", Quantidade = 50, Tamanho = "L" },

                    // --- CONSUMÍVEIS DE INJEÇÃO ---
                    new MaterialEquipamentoAssociado { NomeEquipamento = "Seringa", Quantidade = 150, Tamanho = "5ml" },
                    new MaterialEquipamentoAssociado { NomeEquipamento = "Seringa", Quantidade = 100, Tamanho = "10ml" },
                    new MaterialEquipamentoAssociado { NomeEquipamento = "Agulha Hipodérmica", Quantidade = 200, Tamanho = "21G" },
                    new MaterialEquipamentoAssociado { NomeEquipamento = "Agulha Hipodérmica", Quantidade = 180, Tamanho = "23G" },
                    new MaterialEquipamentoAssociado { NomeEquipamento = "Algodão Hidrófilo (Pacote)", Quantidade = 40, Tamanho = "500g" },

                    // --- CURATIVOS E LIGADURAS ---
                    new MaterialEquipamentoAssociado { NomeEquipamento = "Compressas de Gaze Estéril", Quantidade = 300, Tamanho = "10x10cm" },
                    new MaterialEquipamentoAssociado { NomeEquipamento = "Ligadura Elástica", Quantidade = 60, Tamanho = "5cm" },
                    new MaterialEquipamentoAssociado { NomeEquipamento = "Ligadura Elástica", Quantidade = 50, Tamanho = "10cm" },
                    new MaterialEquipamentoAssociado { NomeEquipamento = "Adesivo Micropore", Quantidade = 80, Tamanho = "2.5cm" },
                    new MaterialEquipamentoAssociado { NomeEquipamento = "Solução Betadine (Frasco)", Quantidade = 30, Tamanho = "100ml" },

                    // --- EQUIPAMENTO DE DIAGNÓSTICO/OUTROS ---
                    new MaterialEquipamentoAssociado { NomeEquipamento = "Espátula de Madeira (Abaixa-língua)", Quantidade = 400, Tamanho = "Único" },
                    new MaterialEquipamentoAssociado { NomeEquipamento = "Copo de Recolha de Urina", Quantidade = 100, Tamanho = "100ml" },
                    new MaterialEquipamentoAssociado { NomeEquipamento = "Gel para Ecografia (Frasco)", Quantidade = 25, Tamanho = "250ml" },
                    new MaterialEquipamentoAssociado { NomeEquipamento = "Papel para marquesa (Rolo)", Quantidade = 40, Tamanho = "Standard" },
                    new MaterialEquipamentoAssociado { NomeEquipamento = "Cateter Intravenoso", Quantidade = 75, Tamanho = "20G" }
                );
                await db.SaveChangesAsync();
            }

            // ==============================================================================
            // 2. SALAS DE EXAME (Atualizado para o novo Modelo com Laboratorio)
            // ==============================================================================
            if (!db.SalaDeExame.Any())
            {
                db.SalaDeExame.AddRange(
                    new SalaDeExames { TipoSala = "Gabinete 01", Laboratorio = "Piso 1 - Consultas" },
                    new SalaDeExames { TipoSala = "Gabinete 02", Laboratorio = "Piso 1 - Consultas" },
                    new SalaDeExames { TipoSala = "Sala Raio-X", Laboratorio = "Piso 0 - Imagiologia" },
                    new SalaDeExames { TipoSala = "Sala TAC", Laboratorio = "Piso 0 - Imagiologia" },
                    new SalaDeExames { TipoSala = "Sala Ressonância", Laboratorio = "Piso -1 - Bunker" },
                    new SalaDeExames { TipoSala = "Sala Gessos", Laboratorio = "Piso 0 - Ortopedia" },
                    new SalaDeExames { TipoSala = "Lab. Colheitas", Laboratorio = "Piso 2 - Laboratórios" },
                    new SalaDeExames { TipoSala = "Box Urgência A", Laboratorio = "Piso 0 - Urgência" },
                    new SalaDeExames { TipoSala = "Box Urgência B", Laboratorio = "Piso 0 - Urgência" },
                    new SalaDeExames { TipoSala = "Sala Pequena Cirurgia", Laboratorio = "Piso 2 - Bloco" }
                );
                await db.SaveChangesAsync();
            }

            // ==============================================================================
            // 3. MÉDICOS (Atualizado para o novo Modelo com string Especialidade)
            // ==============================================================================
            if (!db.Medicos.Any())
            {
                db.Medicos.AddRange(
                    new Medicos { Nome = "Dr. João Silva", Especialidade = "Cardiologia", Telefone = "910000001", Email = "joao.silva@hospital.pt" },
                    new Medicos { Nome = "Dra. Ana Martins", Especialidade = "Clínica Geral", Telefone = "910000002", Email = "ana.martins@hospital.pt" },
                    new Medicos { Nome = "Dr. Pedro Costa", Especialidade = "Ortopedia", Telefone = "910000003", Email = "pedro.costa@hospital.pt" },
                    new Medicos { Nome = "Dra. Sofia Sousa", Especialidade = "Pediatria", Telefone = "910000004", Email = "sofia.sousa@hospital.pt" },
                    new Medicos { Nome = "Dr. Carlos Ferreira", Especialidade = "Neurologia", Telefone = "910000005", Email = "carlos.ferreira@hospital.pt" },
                    new Medicos { Nome = "Dra. Beatriz Lima", Especialidade = "Dermatologia", Telefone = "910000006", Email = "beatriz.lima@hospital.pt" },
                    new Medicos { Nome = "Dr. Tiago Mendes", Especialidade = "Radiologia", Telefone = "910000007", Email = "tiago.mendes@hospital.pt" },
                    new Medicos { Nome = "Dra. Inês Rocha", Especialidade = "Ginecologia", Telefone = "910000008", Email = "ines.rocha@hospital.pt" },
                    new Medicos { Nome = "Dr. Ricardo Lopes", Especialidade = "Urologia", Telefone = "910000009", Email = "ricardo.lopes@hospital.pt" },
                    new Medicos { Nome = "Dra. Clara Nunes", Especialidade = "Oftalmologia", Telefone = "910000010", Email = "clara.nunes@hospital.pt" },
                    new Medicos { Nome = "Dr. Manuel Santos", Especialidade = "Clínica Geral", Telefone = "910000011", Email = "manuel.santos@hospital.pt" },
                    new Medicos { Nome = "Dra. Teresa Bento", Especialidade = "Cardiologia", Telefone = "910000012", Email = "teresa.bento@hospital.pt" }
                );
                await db.SaveChangesAsync();
            }

            // ==============================================================================
            // 4. UTENTES (Atualizado para o novo Modelo Completo)
            // ==============================================================================
            if (!db.Utentes.Any())
            {
                var nomesMasculinos = new[] { "António", "José", "Manuel", "Francisco", "João", "Diogo", "Tiago", "Rui", "Pedro", "Miguel" };
                var nomesFemininos = new[] { "Maria", "Ana", "Isabel", "Joana", "Sofia", "Beatriz", "Mariana", "Inês", "Catarina", "Rita" };
                var apelidos = new[] { "Silva", "Santos", "Ferreira", "Pereira", "Oliveira", "Costa", "Rodrigues", "Martins", "Jesus", "Sousa" };

                var listaUtentes = new List<Utente>();

                // Gerar 25 Utentes
                for (int i = 1; i <= 25; i++)
                {
                    bool isMasc = rnd.Next(2) == 0;
                    string primeiroNome = isMasc
                        ? nomesMasculinos[rnd.Next(nomesMasculinos.Length)]
                        : nomesFemininos[rnd.Next(nomesFemininos.Length)];
                    string apelido = apelidos[rnd.Next(apelidos.Length)] + " " + apelidos[rnd.Next(apelidos.Length)];
                    string nomeCompleto = $"{primeiroNome} {apelido}";

                    listaUtentes.Add(new Utente
                    {
                        UtenteSNS = 100000000 + i,
                        Nif = (200000000 + i).ToString(), // Garante 9 digitos
                        NumCC = $"{10000000 + i}ZZ{i}",
                        Nome = nomeCompleto,
                        Genero = isMasc ? "Masculino" : "Feminino",
                        Morada = $"Rua das Flores nº {rnd.Next(1, 200)}, Portugal",
                        CodigoPostal = $"{rnd.Next(1000, 9999)}-{rnd.Next(100, 999)}",
                        Numero = $"9{rnd.Next(10000000, 99999999)}", // Gera telefone valido pt
                        Email = $"{primeiroNome.ToLower()}.{apelidos[0].ToLower()}{i}@email.com",
                        NomeEmergencia = isMasc ? "Esposa" : "Marido",
                        NumeroEmergencia = 910000000 + i,
                        Estado = "Ativo"
                    });
                }

                db.Utentes.AddRange(listaUtentes);
                await db.SaveChangesAsync();
            }

            // ==============================================================================
            // 5. EXAMES (Geração Massiva)
            // ==============================================================================

            // Só gera se tivermos menos de 100 exames
            if (await db.Exames.CountAsync() < 100)
            {
                var allUtentes = await db.Utentes.ToListAsync();
                var allMedicos = await db.Medicos.ToListAsync();
                var allSalas = await db.SalaDeExame.ToListAsync();

                var allTipos = await db.ExameTipo.ToListAsync();
                var allProfs = await db.ProfissionalExecutante.ToListAsync();
                var allMateriais = await db.MaterialEquipamentoAssociado.ToListAsync();

                if (allUtentes.Any() && allMedicos.Any() && allSalas.Any())
                {
                    var listaExames = new List<Exame>();
                    var notasPossiveis = new[] {
                        "Paciente chegou cedo.",
                        "Utente ansioso.",
                        "Jejum cumprido.",
                        "Urgente.",
                        "Reagendado pelo paciente.",
                        "Exame de rotina.",
                        "Tudo normal.",
                        null, null, null
                    };

                    // GERAR 150 EXAMES
                    for (int i = 0; i < 150; i++)
                    {
                        // Data aleatória: -60 dias a +45 dias
                        int dias = rnd.Next(-60, 46);
                        var dataExame = DateTime.Now.AddDays(dias).AddHours(rnd.Next(-9, 9)).AddMinutes(rnd.Next(0, 4) * 15);

                        // Estado baseado na data
                        string estado;
                        if (dataExame < DateTime.Now)
                            estado = rnd.Next(10) < 8 ? "Realizado" : "Cancelado";
                        else
                            estado = rnd.Next(10) < 7 ? "Marcado" : "Pendente";

                        // Sorteia entidades
                        var utente = allUtentes[rnd.Next(allUtentes.Count)];
                        var medico = allMedicos[rnd.Next(allMedicos.Count)];
                        var sala = allSalas[rnd.Next(allSalas.Count)];

                        // Entidades opcionais (garantir que existem)
                        var tipo = allTipos.Any() ? allTipos[rnd.Next(allTipos.Count)] : null;
                        var prof = allProfs.Any() ? allProfs[rnd.Next(allProfs.Count)] : null;
                        var mat = allMateriais.Any() ? allMateriais[rnd.Next(allMateriais.Count)] : null;

                        if (tipo != null && prof != null && mat != null)
                        {
                            listaExames.Add(new Exame
                            {
                                DataHoraMarcacao = dataExame,
                                Estado = estado,
                                Notas = notasPossiveis[rnd.Next(notasPossiveis.Length)],

                                // Chaves Estrangeiras
                                UtenteId = utente.UtenteId,
                                MedicoSolicitanteId = medico.Id,
                                SalaDeExameId = sala.SalaId,

                                ExameTipoId = tipo.ExameTipoId,
                                ProfissionalExecutanteId = prof.ProfissionalExecutanteId
                            });
                        }
                    }

                    db.Exames.AddRange(listaExames);
                    await db.SaveChangesAsync();
                }
            }
        }

        // --- Identity Helpers (Mantidos iguais) ---
        internal static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = {
                "Rececionista",
                "Utente",
                "Tecnico",
                "Supervisor Tecnico",
                "Gestor",
                "Medico",
                "Admin",
            };
            foreach (var role in roles)
                if (!await roleManager.RoleExistsAsync(role)) await roleManager.CreateAsync(new IdentityRole(role));
        }

        internal static async Task SeedProfissionalLogin(UserManager<IdentityUser> userManager)
        {
            await CriarUser(userManager, "Kandonga123@gmail.com", "Tecnico");
        }
        internal static async Task SeedDefaultAdmin(UserManager<IdentityUser> userManager)
        {
            await CriarUser(userManager, "admin@healthwellbeing.com", "Admin");
        }

        internal static async Task SeedGroupMembers(UserManager<IdentityUser> userManager)
        {
            string[] emails = { "diogomassano@ipg.pt", "dinisgomes@ipg.pt", "rafaelrodrigues@ipg.pt", "joaquimgoncalves@ipg.pt" };
            foreach (var email in emails) await CriarUser(userManager, email, "Admin");
        }

        // Novo método para o João Martins
        internal static async Task SeedRecepcionistaLogin(UserManager<IdentityUser> userManager)
        {
            await CriarUser(userManager, "JoaoMartinsRec@gmail.com", "Rececionista");
            await CriarUser(userManager, "pedro.costa@hospital.pt", "Medico");
            await CriarUser(userManager, "DiogoRodrigues04@gmail.com", "Supervisor Tecnico");
        }
        

        private static async Task CriarUser(UserManager<IdentityUser> um, string email, string role)
        {
            if (await um.FindByNameAsync(email) == null)
            {
                var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                var res = await um.CreateAsync(user, "Secret123$");
                if (res.Succeeded) await um.AddToRoleAsync(user, role);
            }
        }
    }
}
