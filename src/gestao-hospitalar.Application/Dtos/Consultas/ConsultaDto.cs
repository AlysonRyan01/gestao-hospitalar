using gestao_hospitalar.Application.Dtos.Medicos;
using gestao_hospitalar.Application.Dtos.Pacientes;
using gestao_hospitalar.Application.Dtos.Users;
using gestao_hospitalar.Domain.Consultas.Enums;

namespace gestao_hospitalar.Application.Dtos.Consultas;

public record class ConsultaDto(
    Guid Id, 
    string Sobre, 
    DateTime MarcadoPara, 
    DateTime? FinalConsultaPara, 
    string? MotivoCancelamento, 
    EStatusConsulta Status, 
    PacienteDto? Paciente, 
    MedicoDto? Medico);