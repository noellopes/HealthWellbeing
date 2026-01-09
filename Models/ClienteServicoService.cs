using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthWellbeing.Models
{
    public static class ClienteServicoService
    {
        private static List<ClienteServico> servicos = new List<ClienteServico>
        {
            new ClienteServico
            {
                ClienteServicoId = 1,
                NomeCliente = "Maria Silva",
                DataServico = DateTime.Today.AddDays(1),
                TipoTratamento = "Massagem Terapêutica",
                DuracaoMinutos = 60,
                Terapeuta = "João Ferreira",
                Sala = "Sala 1",
                Estado = EstadoServico.Agendado
            },
            new ClienteServico
            {
                ClienteServicoId = 2,
                NomeCliente = "João Pereira",
                DataServico = DateTime.Today.AddDays(2),
                TipoTratamento = "Banho Termal",
                DuracaoMinutos = 45,
                Terapeuta = "Ana Marques",
                Sala = "Sala 3",
                Estado = EstadoServico.Agendado
            },
            new ClienteServico
            {
                ClienteServicoId = 3,
                NomeCliente = "Rita Carvalho",
                DataServico = DateTime.Today.AddDays(-1),
                TipoTratamento = "Fisioterapia em Piscina Termal",
                DuracaoMinutos = 50,
                Terapeuta = "Carlos Santos",
                Sala = "Piscina 2",
                Estado = EstadoServico.Concluido
            },
        new ClienteServico
            {
                ClienteServicoId = 4,
                NomeCliente = "Bruno Costa",
                DataServico = DateTime.Today.AddDays(3),
                TipoTratamento = "Massagem de Relaxamento",
                DuracaoMinutos = 70,
                Terapeuta = "Ana Marques",
                Sala = "Sala 2",
                Estado = EstadoServico.Agendado
            },
            new ClienteServico
            {
                ClienteServicoId = 5,
                NomeCliente = "Sofia Almeida",
                DataServico = DateTime.Today.AddDays(5),
                TipoTratamento = "Terapia com Lama Termal",
                DuracaoMinutos = 40,
                Terapeuta = "João Ferreira",
                Sala = "Sala 5",
                Estado = EstadoServico.Agendado
            },
            new ClienteServico
            {
                ClienteServicoId = 6,
                NomeCliente = "Miguel Rocha",
                DataServico = DateTime.Today.AddDays(-2),
                TipoTratamento = "Banho de Hidromassagem",
                DuracaoMinutos = 45,
                Terapeuta = "Ana Marques",
                Sala = "Sala 4",
                Estado = EstadoServico.Concluido
            },
            new ClienteServico
            {
                ClienteServicoId = 7,
                NomeCliente = "Patrícia Lopes",
                DataServico = DateTime.Today.AddDays(4),
                TipoTratamento = "Inalações Termais",
                DuracaoMinutos = 30,
                Terapeuta = "Carlos Santos",
                Sala = "Sala 6",
                Estado = EstadoServico.Agendado
            },
            new ClienteServico
            {
                ClienteServicoId = 8,
                NomeCliente = "André Marques",
                DataServico = DateTime.Today.AddDays(-3),
                TipoTratamento = "Terapia Respiratória",
                DuracaoMinutos = 50,
                Terapeuta = "Ana Marques",
                Sala = "Sala 3",
                Estado = EstadoServico.Concluido
            },
            new ClienteServico
            {
                ClienteServicoId = 9,
                NomeCliente = "Carla Mendes",
                DataServico = DateTime.Today.AddDays(6),
                TipoTratamento = "Massagem com Pedras Quentes",
                DuracaoMinutos = 60,
                Terapeuta = "João Ferreira",
                Sala = "Sala 1",
                Estado = EstadoServico.Agendado
            },
            new ClienteServico
            {
                ClienteServicoId = 10,
                NomeCliente = "Pedro Nunes",
                DataServico = DateTime.Today.AddDays(7),
                TipoTratamento = "Banho de Vapor Termal",
                DuracaoMinutos = 35,
                Terapeuta = "João Dantas",
                Sala = "Sala 2",
                Estado = EstadoServico.Agendado
            }
        };

        public static List<ClienteServico> GetAll() => servicos;

        public static ClienteServico? GetById(int id) =>
            servicos.FirstOrDefault(s => s.ClienteServicoId == id);
    }
}
