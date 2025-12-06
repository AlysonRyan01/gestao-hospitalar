# Gestão Hospitalar - Documentação

## 1. Entidades Principais

### 1.1 User
Representa o usuário do sistema.

| Campo    | Tipo   | Descrição                          |
|----------|--------|------------------------------------|
| Name     | string | Nome do usuário                     |
| Email    | string | Email de login e contato            |
| Password | string | Senha de acesso                     |
| Phone    | string | Telefone de contato                 |

> Observação: Um usuário pode se tornar um paciente, mas mantém suas credenciais de login separadas.

---

### 1.2 Paciente
Representa o paciente do hospital.

| Campo   | Tipo   | Descrição                                 |
|---------|--------|-------------------------------------------|
| UserId  | Guid   | Referência ao `User` correspondente       |
| User    | User   | Usuário associado                         |

#### Relações
- Um paciente pode ter várias consultas (`Consulta`).

#### Funções principais
- Criar paciente a partir de um `User`.
- Adicionar consulta.
- Visualizar consultas por status:
    - Solicitadas
    - Marcadas
    - Concluídas
    - Canceladas

---

### 1.3 Medico
Representa o médico do hospital.

| Campo        | Tipo   | Descrição                   |
|--------------|--------|-----------------------------|
| Nome         | string | Nome do médico              |
| Telefone     | string | Telefone de contato         |
| Especialidade| string | Especialidade médica        |

#### Relações
- Um médico pode ter várias consultas (`Consulta`).

#### Funções principais
- Criar e atualizar médico.
- Validar horários disponíveis para consultas.
- Visualizar consultas por status:
    - Marcadas
    - Concluídas
    - Canceladas

---

### 1.4 Consulta
Representa uma consulta médica agendada.

| Campo               | Tipo             | Descrição                                           |
|---------------------|-----------------|---------------------------------------------------|
| Paciente            | Paciente        | Paciente associado à consulta                     |
| Medico              | Medico?         | Médico associado (pode ser null inicialmente)    |
| Sobre               | string          | Descrição do problema do paciente                |
| MarcadoPara         | DateTime        | Data e hora marcada para a consulta              |
| FinalConsultaPara   | DateTime?       | Data e hora final da consulta                    |
| MotivoCancelamento  | string?         | Motivo da consulta cancelada                     |
| Status              | EStatusConsulta | Estado da consulta (`Solicitada`, `Marcada`, `Concluída`, `Cancelada`) |

#### Funções principais
- Solicitar agendamento (feito pelo paciente).
- Agendar consulta (feito pelo médico com horário disponível).
- Concluir consulta.
- Cancelar consulta.
- Validar horário da consulta (não permitir fora do expediente ou finais de semana).

---

## 2. Validações

- **Paciente**
    - Nome não pode ser vazio.
    - Email deve seguir padrão válido.
    - Telefone deve seguir padrão válido.
- **Consulta**
    - Só pode ser agendada em dias úteis.
    - Horários válidos: 09:00 às 12:00 e 13:00 às 18:00.
- **Medico**
    - Horários de consulta devem respeitar duração mínima (5 min) e máxima (3 h).
    - Evita conflitos de horários com outras consultas marcadas.

---

## 3. Relacionamentos

- Um `User` pode se tornar um `Paciente`.
- Um `Paciente` pode ter múltiplas `Consultas`.
- Um `Medico` pode ter múltiplas `Consultas`.
- Uma `Consulta` possui exatamente um `Paciente` e pode ter um `Medico` atribuído.

---

## 4. Regras de Negócio

- Paciente só solicita consulta, não agenda diretamente.
- Médico valida horários e agenda consultas.
- Consultas podem ser concluídas ou canceladas.
- Consultas canceladas não podem ser agendadas novamente.

---

## 5. Observações

- Todas as entidades são `AggregateRoot`.
- Métodos de criação (`Criar`) utilizam padrão `Result` para validação.
- Informações sensíveis, como senha, ficam apenas no `User`.
- O `Paciente` armazena dados de contato que podem divergir do `User`.
- Consultas são o ponto central de interação entre paciente e médico.

---

## 6. Testes Unitários

Os testes utilizam **MSTest** e cobrem as principais funcionalidades das entidades do sistema.

### 6.1 User
Testes da criação de usuário e validação de dados:

- **DeveCriarUsuarioComSucesso**: cria um usuário válido e verifica se todos os campos foram atribuídos corretamente.
- **NaoDeveCriarUsuarioSemNome**: falha ao tentar criar usuário sem nome.
- **NaoDeveCriarUsuarioComEmailInvalido**: falha ao tentar criar usuário com e-mail inválido.
- **NaoDeveCriarUsuarioSemSenha**: falha ao tentar criar usuário sem senha.
- **NaoDeveCriarUsuarioComTelefoneInvalido**: falha ao tentar criar usuário com telefone inválido.

---

### 6.2 Paciente
Testes da criação de paciente e gerenciamento de consultas:

- **DeveCriarPacienteComSucesso**: cria um paciente a partir de um usuário existente e verifica o vínculo.
- **DeveAdicionarConsultaAoPaciente**: adiciona uma consulta ao paciente e valida se foi adicionada corretamente.
- **DeveRetornarConsultasPorStatus**: filtra consultas do paciente por status (solicitadas, marcadas, canceladas, concluídas).

---

### 6.3 Medico
Testes da criação de médico e validação de horários:

- **DeveCriarMedicoComSucesso**: cria um médico válido e verifica todos os campos.
- **NaoDeveCriarMedicoSemNome**: falha ao criar médico sem nome.
- **NaoDeveCriarMedicoComTelefoneInvalido**: falha ao criar médico com telefone inválido.
- **NaoDeveCriarMedicoSemEspecialidade**: falha ao criar médico sem especialidade.
- **DeveValidarHorarioDisponivel**: verifica se o médico consegue validar horários livres.
- **NaoDeveValidarHorarioComConflito**: falha ao tentar marcar consulta em horário já ocupado.
- **NaoDeveValidarHorarioForaDuracaoPermitida**: falha ao tentar marcar consulta com duração menor que 5 minutos ou maior que 3 horas.

---

### 6.4 Consulta
Testes do ciclo completo da consulta:

- **DeveCriarConsultaComSucesso**: solicita uma consulta e verifica status e dados.
- **NaoDeveCriarConsultaComHorarioInvalido**: falha ao tentar marcar consulta fora do expediente ou em final de semana.
- **DeveAgendarConsultaComSucesso**: agenda consulta com médico disponível.
- **NaoDeveAgendarConsultaCancelada**: impede agendamento de consulta já cancelada.
- **DeveConcluirConsulta**: conclui uma consulta previamente marcada.
- **NaoDeveConcluirConsultaNaoMarcada**: falha ao tentar concluir consulta não marcada.
- **DeveCancelarConsulta**: cancela uma consulta e armazena motivo.
- **NaoDeveCancelarConsultaSemMotivo**: falha ao tentar cancelar consulta sem informar motivo.
