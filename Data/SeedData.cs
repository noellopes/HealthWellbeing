using HealthWellbeing.Controllers;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthWellbeing.Data
{
    public static class SeedData
    {
        public static void Populate(HealthWellbeingDbContext? dbContext)
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

            // Garante que a BD existe
            dbContext.Database.EnsureCreated();

            // 1. Gêneros (Dependência para Exercícios)
            PopulateGeneros(dbContext);

            // 2. Grupos Musculares e Músculos (Dependência para Exercícios)
            PopulateGruposMusculares(dbContext);

            // 3. Equipamentos (NOVO - Dependência para Exercícios)
            PopulateEquipamentos(dbContext);

            // 4. Exercícios (Depende de 1, 2 e 3)
            // 3. Tipos de Exercício e Benefícios (AGORA CRIAMOS OS BENEFÍCIOS PRIMEIRO!)
            PopulateBeneficios(dbContext); // Deve vir primeiro para ter os IDs
            PopulateTiposExercicio(dbContext); // Depende dos IDs de Benefícios

            // 4. Exercícios (Depende de 1 e 2)
            PopulateExercicios(dbContext);

            // 5. Problemas de Saúde (Independente)
            PopulateProblemasSaude(dbContext);

            // 6. Tipos de Exercício e Benefícios (Independente)
            PopulateTiposExercicio(dbContext);
        }

        private static void PopulateGeneros(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.Genero.Any()) return;

            var generos = new[]
            {
                new Genero { NomeGenero = "Masculino" },
                new Genero { NomeGenero = "Feminino" },
                new Genero { NomeGenero = "Unisexo" }
            };

            dbContext.Genero.AddRange(generos);
            dbContext.SaveChanges();
        }

        private static void PopulateGruposMusculares(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.GrupoMuscular.Any()) return;

            // --- Criação dos Grupos ---
            var grupos = new[]
            {
                new GrupoMuscular { GrupoMuscularNome = "Peito", LocalizacaoCorporal = "Frente do tronco" },
                new GrupoMuscular { GrupoMuscularNome = "Costas", LocalizacaoCorporal = "Parte posterior do tronco" },
                new GrupoMuscular { GrupoMuscularNome = "Bíceps", LocalizacaoCorporal = "Parte frontal do braço" },
                new GrupoMuscular { GrupoMuscularNome = "Tríceps", LocalizacaoCorporal = "Parte posterior do braço" },
                new GrupoMuscular { GrupoMuscularNome = "Ombros", LocalizacaoCorporal = "Deltoides e trapézio" },
                new GrupoMuscular { GrupoMuscularNome = "Pernas", LocalizacaoCorporal = "Quadríceps, isquiotibiais e glúteos" },
                new GrupoMuscular { GrupoMuscularNome = "Abdômen", LocalizacaoCorporal = "Região abdominal" },
                new GrupoMuscular { GrupoMuscularNome = "Panturrilhas", LocalizacaoCorporal = "Região inferior da perna" },
                new GrupoMuscular { GrupoMuscularNome = "Antebraços", LocalizacaoCorporal = "Parte inferior do braço" },
                new GrupoMuscular { GrupoMuscularNome = "Trapézio", LocalizacaoCorporal = "Parte superior das costas e pescoço" },
                new GrupoMuscular { GrupoMuscularNome = "Glúteos", LocalizacaoCorporal = "Região das nádegas" },
                new GrupoMuscular { GrupoMuscularNome = "Adutores", LocalizacaoCorporal = "Parte interna das coxas" },
                new GrupoMuscular { GrupoMuscularNome = "Abdutores", LocalizacaoCorporal = "Parte externa das coxas" },
                new GrupoMuscular { GrupoMuscularNome = "Serrátil Anterior", LocalizacaoCorporal = "Lateral do tórax" },
                new GrupoMuscular { GrupoMuscularNome = "Reto Femoral", LocalizacaoCorporal = "Parte frontal da coxa" }
            };

            dbContext.GrupoMuscular.AddRange(grupos);
            dbContext.SaveChanges();

            // --- Criação dos Músculos ---
            var gruposDb = dbContext.GrupoMuscular.ToList();
            Func<string, int> getId = nome => gruposDb.First(g => g.GrupoMuscularNome == nome).GrupoMuscularId;

            var musculos = new[]
            {
                new Musculo { Nome_Musculo = "Peitoral Maior", GrupoMuscularId = getId("Peito") },
                new Musculo { Nome_Musculo = "Peitoral Menor", GrupoMuscularId = getId("Peito") },
                new Musculo { Nome_Musculo = "Dorsal Largo", GrupoMuscularId = getId("Costas") },
                new Musculo { Nome_Musculo = "Romboides", GrupoMuscularId = getId("Costas") },
                new Musculo { Nome_Musculo = "Bíceps Braquial", GrupoMuscularId = getId("Bíceps") },
                new Musculo { Nome_Musculo = "Tríceps Braquial", GrupoMuscularId = getId("Tríceps") },
                new Musculo { Nome_Musculo = "Deltoide Anterior", GrupoMuscularId = getId("Ombros") },
                new Musculo { Nome_Musculo = "Deltoide Lateral", GrupoMuscularId = getId("Ombros") },
                new Musculo { Nome_Musculo = "Quadríceps", GrupoMuscularId = getId("Pernas") },
                new Musculo { Nome_Musculo = "Isquiotibiais", GrupoMuscularId = getId("Pernas") },
                new Musculo { Nome_Musculo = "Glúteo Máximo", GrupoMuscularId = getId("Glúteos") },
                new Musculo { Nome_Musculo = "Reto Abdominal", GrupoMuscularId = getId("Abdômen") },
                new Musculo { Nome_Musculo = "Gastrocnêmio", GrupoMuscularId = getId("Panturrilhas") }
            };

            dbContext.Musculo.AddRange(musculos);
            dbContext.SaveChanges();
        }

        private static void PopulateEquipamentos(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.Equipamento.Any()) return;

            var equipamentos = new[]
            {
                new Equipamento { NomeEquipamento = "Halteres" },
                new Equipamento { NomeEquipamento = "Barra Olímpica" },
                new Equipamento { NomeEquipamento = "Banco de Musculação" },
                new Equipamento { NomeEquipamento = "Tapete de Yoga" },
                new Equipamento { NomeEquipamento = "Bola de Pilates" },
                new Equipamento { NomeEquipamento = "Elásticos de Resistência" },
                new Equipamento { NomeEquipamento = "Passadeira" },
                new Equipamento { NomeEquipamento = "Bicicleta Estática" },
                new Equipamento { NomeEquipamento = "Kettlebell" }
            };

            dbContext.Equipamento.AddRange(equipamentos);
            dbContext.SaveChanges();
        }

        private static void PopulateProblemasSaude(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.ProblemaSaude.Any()) return;

            var problemas = new[]
            {
                new ProblemaSaude { ProblemaCategoria = "Muscular", ProblemaNome = "Tendinite", ZonaAtingida = "Braço direito", Gravidade = 6 },
                new ProblemaSaude { ProblemaCategoria = "Cardíaco", ProblemaNome = "Hipertensão Arterial", ZonaAtingida = "Coração", Gravidade = 8 },
                new ProblemaSaude { ProblemaCategoria = "Respiratório", ProblemaNome = "Asma", ZonaAtingida = "Pulmões", Gravidade = 5 },
                new ProblemaSaude { ProblemaCategoria = "Articular", ProblemaNome = "Artrite", ZonaAtingida = "Joelhos", Gravidade = 7 },
                new ProblemaSaude { ProblemaCategoria = "Neurológico", ProblemaNome = "Enxaqueca Crónica", ZonaAtingida = "Cabeça", Gravidade = 6 },
                new ProblemaSaude { ProblemaCategoria = "Postural", ProblemaNome = "Escoliose", ZonaAtingida = "Coluna Vertebral", Gravidade = 5 },
                new ProblemaSaude { ProblemaCategoria = "Endócrino", ProblemaNome = "Diabetes Tipo 2", ZonaAtingida = "Sistema Endócrino", Gravidade = 8 },
                new ProblemaSaude { ProblemaCategoria = "Ocular", ProblemaNome = "Miopia", ZonaAtingida = "Olhos", Gravidade = 3 }
            };

            dbContext.ProblemaSaude.AddRange(problemas);
            dbContext.SaveChanges();
        }

        private static void PopulateBeneficios(HealthWellbeingDbContext dbContext)
        {
            // Usa ToHashSet para verificação eficiente
            var beneficiosExistentes = dbContext.Beneficio.Select(b => b.NomeBeneficio).ToHashSet();

            var todosBeneficios = new[]
            {
                new Beneficio { NomeBeneficio = "Melhora da resistência cardiovascular", DescricaoBeneficio = "Aumenta a capacidade do coração e pulmões de fornecer oxigénio aos músculos." },
                new Beneficio { NomeBeneficio = "Fortalecimento muscular", DescricaoBeneficio = "Aumenta a força, resistência e tamanho das fibras musculares." },
                new Beneficio { NomeBeneficio = "Aumento da flexibilidade", DescricaoBeneficio = "Melhora a amplitude de movimento nas articulações, prevenindo lesões." },
                new Beneficio { NomeBeneficio = "Redução do stress", DescricaoBeneficio = "Ajuda a reduzir a ansiedade e melhora o humor através da libertação de endorfinas." },
                new Beneficio { NomeBeneficio = "Perda de peso", DescricaoBeneficio = "Auxilia no controle do peso, aumentando o gasto calórico e melhorando o metabolismo." },
                new Beneficio { NomeBeneficio = "Melhora da coordenação e equilíbrio", DescricaoBeneficio = "Desenvolve a capacidade do corpo de realizar movimentos complexos e manter a postura." },
                new Beneficio { NomeBeneficio = "Aumento da densidade óssea", DescricaoBeneficio = "Estimula o aumento da massa óssea, prevenindo a osteoporose." },
                
                // 8 Novos Benefícios
                new Beneficio { NomeBeneficio = "Melhoria da Qualidade do Sono", DescricaoBeneficio = "Regula os ciclos de sono, ajudando a adormecer mais rápido e ter um descanso mais profundo." },
                new Beneficio { NomeBeneficio = "Aumento da Capacidade Pulmonar", DescricaoBeneficio = "Fortalece os músculos respiratórios e aumenta a eficiência da troca gasosa." },
                new Beneficio { NomeBeneficio = "Controle Glicémico", DescricaoBeneficio = "Ajuda a regular os níveis de açúcar no sangue, aumentando a sensibilidade à insulina." },
                new Beneficio { NomeBeneficio = "Promoção da Saúde Mental", DescricaoBeneficio = "Combate sintomas de depressão e melhora o foco cognitivo." },
                new Beneficio { NomeBeneficio = "Recuperação Ativa", DescricaoBeneficio = "Acelera a eliminação de ácido lático e reduz a dor muscular pós-exercício." },
                new Beneficio { NomeBeneficio = "Melhora da Postura Corporal", DescricaoBeneficio = "Fortalece os músculos do tronco e corrige desalinhamentos posturais." },
                new Beneficio { NomeBeneficio = "Estímulo do Sistema Imunitário", DescricaoBeneficio = "Aumenta a circulação de células imunitárias, fortalecendo a defesa do organismo." },
                new Beneficio { NomeBeneficio = "Desenvolvimento de Agilidade", DescricaoBeneficio = "Aumenta a capacidade de mudar rapidamente a direção e velocidade do movimento." }
            };

            // Filtra e adiciona apenas os benefícios que ainda não existem
            var beneficiosParaAdicionar = todosBeneficios
                .Where(b => !beneficiosExistentes.Contains(b.NomeBeneficio))
                .ToList();

            if (beneficiosParaAdicionar.Any())
            {
                dbContext.Beneficio.AddRange(beneficiosParaAdicionar);
                dbContext.SaveChanges();
            }
        }

        private static void PopulateTiposExercicio(HealthWellbeingDbContext dbContext)
        {
            // 1. Recuperar Benefícios para ligar
            // É essencial recarregar a BD para obter os IDs gerados pelo SaveChanges do PopulateBeneficios.
            var beneficiosDb = dbContext.Beneficio.ToDictionary(b => b.NomeBeneficio, b => b);

            // Função de utilidade para buscar o ID do benefício pela chave (nome).
            // Usaremos o ID na entidade de junção (TipoExercicioBeneficio) para evitar o erro de tracking.
            Beneficio GetBen(string nome)
            {
                if (beneficiosDb.TryGetValue(nome, out var beneficio))
                {
                    return beneficio;
                }
                // Fallback (se por acaso o nome não for encontrado, o que não deve acontecer)
                return beneficiosDb.First().Value;
            }

            // 2. Definir todos os tipos de exercício
            var todosTiposExercicio = new[]
            {
                new TipoExercicio
                {
                    NomeTipoExercicios = "Cardiovascular",
                    DescricaoTipoExercicios = "Atividades contínuas que aumentam a frequência cardíaca por um período prolongado.",
                    CaracteristicasTipoExercicios = "Alta queima calórica, ritmo constante, como corrida ou ciclismo.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        // CORREÇÃO: Usar o ID da entidade já rastreada
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Melhora da resistência cardiovascular").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Perda de peso").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Aumento da Capacidade Pulmonar").BeneficioId }
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Força",
                    DescricaoTipoExercicios = "Exercícios focados no desenvolvimento da força e massa muscular.",
                    CaracteristicasTipoExercicios = "Uso de pesos livres, máquinas ou resistência corporal, com repetições e séries.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Fortalecimento muscular").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Aumento da densidade óssea").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Controle Glicémico").BeneficioId }
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Flexibilidade",
                    DescricaoTipoExercicios = "Atividades estáticas ou dinâmicas que melhoram o alongamento muscular e a mobilidade articular.",
                    CaracteristicasTipoExercicios = "Movimentos suaves e sustentados, como alongamentos estáticos.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Aumento da flexibilidade").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Redução do stress").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Recuperação Ativa").BeneficioId }
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Funcional",
                    DescricaoTipoExercicios = "Exercícios que preparam o corpo para atividades da vida diária, envolvendo múltiplos grupos musculares.",
                    CaracteristicasTipoExercicios = "Trabalho com peso corporal, bolas medicinais ou faixas de resistência, focando na estabilidade.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Fortalecimento muscular").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Melhora da coordenação e equilíbrio").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Melhora da Postura Corporal").BeneficioId }
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Pilates",
                    DescricaoTipoExercicios = "Método focado no fortalecimento do 'core' (centro do corpo), postura e consciência corporal.",
                    CaracteristicasTipoExercicios = "Controle de respiração e movimentos precisos, uso de máquinas ou tapetes.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Fortalecimento muscular").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Melhora da Postura Corporal").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Aumento da flexibilidade").BeneficioId }
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Yoga",
                    DescricaoTipoExercicios = "Prática que combina posturas físicas (asanas), exercícios de respiração (pranayama) e meditação.",
                    CaracteristicasTipoExercicios = "Série de posturas fluidas ou mantidas, com foco na mente e corpo.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Aumento da flexibilidade").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Redução do stress").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Promoção da Saúde Mental").BeneficioId }
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "HIIT",
                    DescricaoTipoExercicios = "Alterna curtos períodos de exercício intenso com períodos de recuperação.",
                    CaracteristicasTipoExercicios = "Esforço máximo seguido de descanso, treino rápido e eficiente.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Melhora da resistência cardiovascular").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Perda de peso").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Aumento da Capacidade Pulmonar").BeneficioId }
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Natação",
                    DescricaoTipoExercicios = "Exercício de baixo impacto que trabalha o corpo inteiro na água.",
                    CaracteristicasTipoExercicios = "Resistência da água, trabalho corporal total, melhora respiratória.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Melhora da resistência cardiovascular").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Fortalecimento muscular").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Aumento da Capacidade Pulmonar").BeneficioId }
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Boxe/Artes Marciais",
                    DescricaoTipoExercicios = "Treino que combina movimentos aeróbicos, de força, agilidade e coordenação.",
                    CaracteristicasTipoExercicios = "Golpes, chutes, uso de sacos de pancada, alta intensidade.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Desenvolvimento de Agilidade").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Redução do stress").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Perda de peso").BeneficioId }
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Calistenia",
                    DescricaoTipoExercicios = "Utiliza o peso corporal como resistência para desenvolver força e coordenação.",
                    CaracteristicasTipoExercicios = "Flexões, barras, agachamentos sem peso adicional, foco em movimentos compostos.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Fortalecimento muscular").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Melhora da coordenação e equilíbrio").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Melhora da Postura Corporal").BeneficioId }
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Treino de Agilidade",
                    DescricaoTipoExercicios = "Focado em melhorar a capacidade de reação, velocidade e coordenação motora.",
                    CaracteristicasTipoExercicios = "Uso de escadas de agilidade, cones, saltos laterais.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Desenvolvimento de Agilidade").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Melhora da coordenação e equilíbrio").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Estímulo do Sistema Imunitário").BeneficioId }
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Caminhada/Passeio",
                    DescricaoTipoExercicios = "Atividade cardiovascular de baixo impacto, ideal para recuperação ou iniciantes.",
                    CaracteristicasTipoExercicios = "Ritmo moderado, pode ser feita em qualquer lugar.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Melhora da resistência cardiovascular").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Redução do stress").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Recuperação Ativa").BeneficioId }
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Recuperação Ativa",
                    DescricaoTipoExercicios = "Movimentos leves destinados a melhorar a circulação e acelerar a recuperação muscular.",
                    CaracteristicasTipoExercicios = "Alongamentos suaves, massagem com rolo de espuma, ioga regenerativa.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Recuperação Ativa").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Aumento da flexibilidade").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Promoção da Saúde Mental").BeneficioId }
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Mindfulness e Respiração",
                    DescricaoTipoExercicios = "Focado na atenção plena e técnicas de respiração para controlar o sistema nervoso.",
                    CaracteristicasTipoExercicios = "Sessões de meditação, exercícios de respiração diafragmática.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Redução do stress").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Promoção da Saúde Mental").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Melhoria da Qualidade do Sono").BeneficioId }
                    }
                }
            };

            // 3. Verifica e adiciona apenas os Tipos de Exercício que faltam
            var tiposExistentes = dbContext.TipoExercicio.Select(t => t.NomeTipoExercicios).ToHashSet();

            var tiposParaAdicionar = todosTiposExercicio
                .Where(t => !tiposExistentes.Contains(t.NomeTipoExercicios))
                .ToList();

            if (tiposParaAdicionar.Any())
            {
                // NOTA: Ao adicionar o TipoExercicio, as suas entidades aninhadas TipoExercicioBeneficio
                // serão rastreadas, mas como apenas usamos o ID do Beneficio, não haverá conflito.
                dbContext.TipoExercicio.AddRange(tiposParaAdicionar);
                dbContext.SaveChanges();
            }
        }

        private static void PopulateExercicios(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.Exercicio.Any()) return;

            // 1. Carregar referências para chaves estrangeiras
            var generos = dbContext.Genero.ToDictionary(g => g.NomeGenero, g => g.GeneroId);
            var grupos = dbContext.GrupoMuscular.ToDictionary(g => g.GrupoMuscularNome, g => g.GrupoMuscularId);
            var equipamentos = dbContext.Equipamento.ToDictionary(e => e.NomeEquipamento, e => e.EquipamentoId);
            var tipos = dbContext.TipoExercicio.ToDictionary(t => t.NomeTipoExercicios, t => t.TipoExercicioId);
            var problemas = dbContext.ProblemaSaude.ToDictionary(p => p.ProblemaNome, p => p.ProblemaSaudeId);

            // Helpers seguros (Fallback para o primeiro item se não encontrar)
            int GetGenId(string nome) => generos.TryGetValue(nome, out int id) ? id : generos.Values.First();
            int GetGrId(string nome) => grupos.TryGetValue(nome, out int id) ? id : grupos.Values.First();
            int GetEqId(string nome) => equipamentos.TryGetValue(nome, out int id) ? id : equipamentos.Values.First();
            // Fallback para "Força" se não encontrar o tipo, ou o primeiro
            int GetTipoId(string nome) => tipos.TryGetValue(nome, out int id) ? id : (tipos.TryGetValue("Força", out int fId) ? fId : tipos.Values.First());
            int GetProbId(string nome) => problemas.TryGetValue(nome, out int id) ? id : 0;

            var exercicios = new List<Exercicio>
            {
                new Exercicio
                {
                    ExercicioNome = "Flexão",
                    Descricao = "Exercício para peito, braços e ombros usando apenas o peso do corpo.",
                    Duracao = 10,
                    Intencidade = 6,
                    CaloriasGastas = 50,
                    Instrucoes = "Deitar no chão, mãos alinhadas aos ombros, flexionar braços mantendo corpo reto.",
                    Repeticoes = 15,
                    Series = 3,
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Unisexo") } },
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Peito") },
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Tríceps") },
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Ombros") }
                    },
                    ExercicioEquipamentos = new List<ExercicioEquipamento> { new ExercicioEquipamento { EquipamentoId = GetEqId("Tapete de Yoga") } },
                    // Contraindicação: Quem tem tendinite no braço
                    Contraindicacoes = (GetProbId("Tendinite") > 0)
                        ? new List<ExercicioProblemaSaude> { new ExercicioProblemaSaude { ProblemaSaudeId = GetProbId("Tendinite") } }
                        : new List<ExercicioProblemaSaude>()
                },
                new Exercicio
                {
                    ExercicioNome = "Agachamento",
                    Descricao = "Exercício composto para membros inferiores.",
                    Duracao = 15,
                    Intencidade = 7,
                    CaloriasGastas = 80,
                    Instrucoes = "Ficar em pé, afastar pernas, flexionar joelhos mantendo costas retas.",
                    Repeticoes = 20,
                    Series = 4,
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Unisexo") } },
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Pernas") },
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Glúteos") }
                    },
                    ExercicioEquipamentos = new List<ExercicioEquipamento>(), // Sem equipamento
                    // Contraindicação: Artrite nos joelhos
                    Contraindicacoes = (GetProbId("Artrite") > 0)
                        ? new List<ExercicioProblemaSaude> { new ExercicioProblemaSaude { ProblemaSaudeId = GetProbId("Artrite") } }
                        : new List<ExercicioProblemaSaude>()
                },
                new Exercicio
                {
                    ExercicioNome = "Prancha",
                    Descricao = "Exercício isométrico para core.",
                    Duracao = 5,
                    Intencidade = 5,
                    CaloriasGastas = 30,
                    Instrucoes = "Apoiar antebraços e ponta dos pés, mantendo corpo reto.",
                    Repeticoes = 1,
                    Series = 3,
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Unisexo") } },
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular> { new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Abdômen") } },
                    ExercicioEquipamentos = new List<ExercicioEquipamento> { new ExercicioEquipamento { EquipamentoId = GetEqId("Tapete de Yoga") } },
                    Contraindicacoes = new List<ExercicioProblemaSaude>()
                },
                new Exercicio
                {
                    ExercicioNome = "Remada com Halteres",
                    Descricao = "Exercício para costas.",
                    Duracao = 12,
                    Intencidade = 7,
                    CaloriasGastas = 70,
                    Instrucoes = "Inclinar tronco, puxar halteres em direção ao abdómen.",
                    Repeticoes = 12,
                    Series = 3,
                    ExercicioGeneros = new List<ExercicioGenero>
                    {
                        new ExercicioGenero { GeneroId = GetGenId("Masculino") },
                        new ExercicioGenero { GeneroId = GetGenId("Feminino") }
                    },
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Costas") },
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Bíceps") }
                    },
                    ExercicioEquipamentos = new List<ExercicioEquipamento> { new ExercicioEquipamento { EquipamentoId = GetEqId("Halteres") } },
                    // Contraindicação: Escoliose (pode agravar se má postura)
                    Contraindicacoes = (GetProbId("Escoliose") > 0)
                        ? new List<ExercicioProblemaSaude> { new ExercicioProblemaSaude { ProblemaSaudeId = GetProbId("Escoliose") } }
                        : new List<ExercicioProblemaSaude>()
                },
                new Exercicio
                {
                    ExercicioNome = "Corrida no Lugar",
                    Descricao = "Cardio para queimar calorias.",
                    Duracao = 20,
                    Intencidade = 6, // No original era 6, ajustei para 8 em alguns exemplos, mantendo 6
                    CaloriasGastas = 150,
                    Instrucoes = "Correr no mesmo lugar elevando os joelhos.",
                    Repeticoes = 1,
                    Series = 1,
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Unisexo") } },
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Pernas") },
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Panturrilhas") }
                    },
                    ExercicioEquipamentos = new List<ExercicioEquipamento>(),
                    // Contraindicação: Asma (se alta intensidade)
                    Contraindicacoes = (GetProbId("Asma") > 0)
                        ? new List<ExercicioProblemaSaude> { new ExercicioProblemaSaude { ProblemaSaudeId = GetProbId("Asma") } }
                        : new List<ExercicioProblemaSaude>()
                },
                new Exercicio
                {
                    ExercicioNome = "Elevação Pélvica",
                    Descricao = "Focado em glúteos.",
                    Duracao = 10,
                    Intencidade = 5,
                    CaloriasGastas = 40,
                    Instrucoes = "Deitar, elevar a bacia contraindo glúteos.",
                    Repeticoes = 15,
                    Series = 3,
                    ExercicioGeneros = new List<ExercicioGenero>
                    {
                        new ExercicioGenero { GeneroId = GetGenId("Feminino") },
                        new ExercicioGenero { GeneroId = GetGenId("Unisexo") }
                    },
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Glúteos") },
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Pernas") }
                    },
                    ExercicioEquipamentos = new List<ExercicioEquipamento> { new ExercicioEquipamento { EquipamentoId = GetEqId("Tapete de Yoga") } },
                    Contraindicacoes = new List<ExercicioProblemaSaude>()
                },
                new Exercicio
                {
                    ExercicioNome = "Supino Reto",
                    Descricao = "Força para peitoral.",
                    Duracao = 15,
                    Intencidade = 8,
                    CaloriasGastas = 90,
                    Instrucoes = "Empurrar barra para cima deitado no banco.",
                    Repeticoes = 10,
                    Series = 4,
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Masculino") } },
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Peito") },
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Tríceps") }
                    },
                    ExercicioEquipamentos = new List<ExercicioEquipamento>
                    {
                        new ExercicioEquipamento { EquipamentoId = GetEqId("Barra Olímpica") },
                        new ExercicioEquipamento { EquipamentoId = GetEqId("Banco de Musculação") }
                    },
                    // Contraindicação: Tendinite e Hipertensão
                    Contraindicacoes = new List<ExercicioProblemaSaude>()
                },
                new Exercicio
                {
                    ExercicioNome = "Abdominal Crunch",
                    Descricao = "Abdómen superior.",
                    Duracao = 7,
                    Intencidade = 5,
                    CaloriasGastas = 35,
                    Instrucoes = "Deitar, levantar ombros do chão.",
                    Repeticoes = 20,
                    Series = 3,
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Unisexo") } },
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular> { new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Abdômen") } },
                    ExercicioEquipamentos = new List<ExercicioEquipamento> { new ExercicioEquipamento { EquipamentoId = GetEqId("Tapete de Yoga") } },
                    Contraindicacoes = new List<ExercicioProblemaSaude>()
                }
            };

            // Lógica extra para adicionar múltiplas contraindicações ao Supino se existirem
            var supino = exercicios.FirstOrDefault(e => e.ExercicioNome == "Supino Reto");
            if (supino != null)
            {
                if (GetProbId("Tendinite") > 0) supino.Contraindicacoes.Add(new ExercicioProblemaSaude { ProblemaSaudeId = GetProbId("Tendinite") });
                if (GetProbId("Hipertensão Arterial") > 0) supino.Contraindicacoes.Add(new ExercicioProblemaSaude { ProblemaSaudeId = GetProbId("Hipertensão Arterial") });
            }

            dbContext.Exercicio.AddRange(exercicios);
            dbContext.SaveChanges();
        }

        public static class Roles
        {
            public const string Administrador = "Administrador";
            public const string Profissional = "ProfissionalSaude";
            public const string Utente = "Utente";
        }

        // Método para criar as Roles (Já tinhas, está correto)
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { Roles.Administrador, Roles.Profissional, Roles.Utente };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        // A. ADMIN
        public static async Task SeedDefaultAdmin(UserManager<IdentityUser> userManager)
        {
            var adminEmail = "admin@ginasio.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                // A password deve ser forte
                var result = await userManager.CreateAsync(adminUser, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, Roles.Administrador);
                }
            }
        }

        public static async Task SeedProfissional(
        UserManager<IdentityUser> userManager, // Liga à BD de Users
        HealthWellbeingDbContext dbContext)    // Liga à BD de Negócio
        {
            // 1. Criar o Login na BD "HealthWellbeingUsers"
            var profEmail = "medico@ginasio.com";
            var profUser = await userManager.FindByEmailAsync(profEmail);

            if (profUser == null)
            {
                profUser = new IdentityUser
                {
                    UserName = profEmail,
                    Email = profEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(profUser, "Medico@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(profUser, Roles.Profissional);
                }
            }

            // Importante: Recarregar o user para garantir que temos o ID correto
            profUser = await userManager.FindByEmailAsync(profEmail);

            // 2. Criar o Perfil na BD "HealthWellbeing"
            // Usamos o UserId (string) para fazer a ponte
            var perfilExiste = await dbContext.Profissional.AnyAsync(p => p.UserId == profUser.Id);

            if (!perfilExiste)
            {
                var novoProfissional = new Profissional
                {
                    UserId = profUser.Id, //ID da outra BD
                    Nome = "Dr. Exemplo",
                    Especialidade = "Fisioterapeuta",
                    NumeroCedula = "12345XYZ"
                };

                dbContext.Profissional.Add(novoProfissional);
                await dbContext.SaveChangesAsync();
            }
        }

        public static async Task SeedUtente(UserManager<IdentityUser> userManager, HealthWellbeingDbContext dbContext)
        {
            var utenteEmail = "utente@ginasio.com";
            var utenteUser = await userManager.FindByEmailAsync(utenteEmail);

            // 1. Criar Login
            if (utenteUser == null)
            {
                utenteUser = new IdentityUser
                {
                    UserName = utenteEmail,
                    Email = utenteEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(utenteUser, "Utente@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(utenteUser, Roles.Utente);
                }
            }
        }
    }
}