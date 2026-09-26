# HavanTestTech

Solução do teste técnico para a vaga de desenvolvedor na Havan Labs. O repositório está organizado em três partes: uma questão teórica, exercícios de algoritmos e manipulação de strings em C#, e uma API de gerenciamento de tarefas construída com Clean Architecture (Domain, Application, Infrastructure e Api), com um frontend em React como diferencial opcional.

## Estrutura do repositório

```
HavanTestTech/
├── src/
│   ├── HavanTestTech.Exercises/        # Parte 2: Questões 2, 3 e 4
│   ├── HavanTestTech.Domain/           # Parte 3: entidade e regras de negócio
│   ├── HavanTestTech.Application/      # Parte 3: casos de uso e contratos
│   ├── HavanTestTech.Infrastructure/   # Parte 3: persistência in-memory
│   └── HavanTestTech.Api/              # Parte 3: Minimal API
├── tests/
│   └── HavanTestTech.Tests/
│       ├── Domain/
│       ├── Application/
│       └── Questions/
└── frontend/                           # Bônus: React + TypeScript
```

## Como rodar

**Requisitos:** .NET 8 SDK e Node 18+.

```bash
# Parte 2 — exercícios de console
dotnet run --project src/HavanTestTech.Exercises

# Testes
dotnet test

# Parte 3 — API
dotnet run --project src/HavanTestTech.Api --urls http://localhost:5080

# Documentação interativa (com a API rodando)
# http://localhost:5080/swagger

# Bônus — frontend (com a API já rodando em outro terminal)
cd frontend
npm install
npm run dev
```

O frontend sobe em `http://localhost:5173` e consome a API em `http://localhost:5080`.

---

## Parte 1: Questão Teórica

### Value Types vs Reference Types — impacto em memória

A diferença fundamental é **onde e como o dado é armazenado**, não o tipo de dado em si.

Um **value type** (`struct`, `int`, `bool`, `enum`, `DateTime`, `decimal`) guarda o valor diretamente na variável. Quando esse valor é atribuído a outra variável ou passado como parâmetro, o runtime copia o valor inteiro — as duas variáveis passam a ser independentes. Quando um value type é uma variável local ou parâmetro de método, ele fica na **Stack**, que é uma região de memória extremamente rápida de alocar e liberar: a liberação acontece automaticamente quando o método retorna, sem qualquer intervenção do Garbage Collector.

Um **reference type** (`class`, `string`, `array`, `delegate`) guarda, na variável, apenas um ponteiro para onde o objeto realmente vive, que é a **Heap**. Copiar a variável copia só o ponteiro — as duas variáveis passam a apontar para o mesmo objeto, e alterar uma reflete na outra. A Heap exige que o Garbage Collector rastreie quando um objeto não tem mais nenhuma referência apontando para ele, para então liberar a memória.

Isso tem uma consequência prática direta no projeto: `TodoStatus` é um `enum` (value type) porque representa um valor simples e imutável — comparar dois `TodoStatus` é comparar dois números inteiros, sem alocação nenhuma. Já `TodoItem` é uma `class` (reference type) porque representa uma entidade com identidade própria: duas instâncias com os mesmos dados ainda são conceitualmente "tarefas diferentes", e o objeto muda de estado ao longo do tempo (por exemplo, quando `ChangeStatus` é chamado) — isso exige referência compartilhada, não cópia.

Um detalhe importante: um value type deixa de estar na Stack quando é campo de uma classe. Por exemplo, `TodoItem.CreatedAt` é um `DateTime` (value type), mas como `TodoItem` é uma classe, esse `DateTime` vive dentro do objeto `TodoItem`, que está na Heap.

### Interface vs Classe Abstrata

Uma **interface** define um contrato — um conjunto de métodos e propriedades que uma classe se compromete a implementar — sem carregar estado nem implementação própria (fora os default interface methods, um recurso mais recente do C#, que não usei aqui). Uma classe pode implementar quantas interfaces quiser.

Uma **classe abstrata** pode ter estado (campos), construtores e métodos com implementação real, além de membros abstratos que as subclasses são obrigadas a implementar. Uma classe só pode herdar de uma única classe abstrata.

A escolha entre uma e outra depende da pergunta: existe comportamento ou estado genuinamente compartilhado entre os tipos, ou eles só precisam concordar em um contrato?

No projeto, `ITodoRepository` é uma interface, não uma classe abstrata, porque a persistência pode ter implementações completamente diferentes entre si — uma lista em memória (`InMemoryTodoRepository`), futuramente Entity Framework Core, ou até uma chamada a um serviço externo. Essas implementações não compartilham nenhum código entre si, só o contrato (`AddAsync`, `GetByIdAsync`, etc.). Usar uma classe abstrata aqui forçaria uma hierarquia artificial e ainda impediria que a implementação de infraestrutura herdasse de outra coisa, já que C# não permite herança múltipla.

Um exemplo em que eu escolheria classe abstrata: se o sistema tivesse vários tipos de notificação (`EmailNotification`, `SmsNotification`, `PushNotification`) que compartilhassem lógica de retry e logging, mas cada um implementasse o envio de forma diferente, uma classe abstrata `NotificationBase` com um método `Send` abstrato e um método `SendWithRetry` já implementado evitaria duplicar a lógica de retry em cada subclasse.

### async/await

`async/await` é açúcar sintático sobre a `Task` (e `Task<T>`): o compilador transforma o método `async` em uma máquina de estados que pode pausar em cada `await` e retomar mais tarde, sem exigir que o desenvolvedor escreva callbacks manualmente.

Na prática, quando o `await` está esperando uma operação de I/O genuína (uma chamada de rede, leitura de disco, consulta a um banco), a thread que estava executando o método **não fica bloqueada esperando**. Ela é devolvida ao ThreadPool e pode atender outra requisição enquanto isso. Quando a operação de I/O termina — avisada pelo sistema operacional —, o restante do método é agendado para rodar, possivelmente em outra thread do pool, retomando exatamente de onde parou.

Isso é o que permite que uma Web API atenda milhares de requisições simultâneas com um número pequeno de threads: em vez de cada requisição prender uma thread até terminar, as threads ficam livres para trabalhar em outras requisições durante a espera por I/O.

Um ponto que vale destacar, porque aparece diretamente no código deste repositório: `async/await` **não é sinônimo de multithreading**. Em `InMemoryTodoRepository`, os métodos são declarados como `Task`/`Task<T>` para respeitar o contrato de `ITodoRepository`, mas como não existe I/O real (é uma lista em memória), eles retornam com `Task.FromResult(...)` ou `Task.CompletedTask` — não há nenhuma thread sendo liberada ou bloqueada, porque não há nada para esperar. A assinatura assíncrona existe para que trocar essa implementação por uma que use um banco de dados real (onde `await` passaria a ter efeito genuíno) não exija mudar o contrato nem o código que o consome.

---

## Parte 2: Lógica de Programação

Código em `src/HavanTestTech.Exercises/`, testes em `tests/HavanTestTech.Tests/Questions/`.

### Questão 2 — Sequência consecutiva

`ConsecutiveSequenceFinder.FindLongest` usa um `HashSet<int>` em vez de ordenar a lista, o que reduz a complexidade de O(n log n) para **O(n)**. A ideia: só expande a contagem a partir de números que são o **início** de uma sequência (isto é, `numero - 1` não está no conjunto). Isso garante que cada elemento é visitado no máximo duas vezes no total, mantendo o algoritmo linear.

```
Entrada: [100, 4, 200, 1, 3, 2]
Saída:   [1, 2, 3, 4] (Tamanho 4)
```

### Questão 3 — Análise de string

`TextSanitizer.Sanitize` normaliza o texto para minúsculas, decompõe caracteres acentuados via `NormalizationForm.FormD` (que separa `á` em `a` + marca de acento) e descarta tudo que não for letra ou dígito — incluindo a marca de acento isolada.

`PhraseAnalyzer.Analyze` conta a frequência de cada caractere em uma única passada e localiza o primeiro caractere não repetido varrendo o texto já higienizado (a ordem é preservada, então o primeiro caractere com contagem 1 no texto higienizado corresponde ao primeiro na frase original).

**Nota sobre o exemplo do enunciado:** ao recontar manualmente `"abateriadocomputadorestafraca"`, os valores batem diferente do exemplo fornecido. A letra `'a'` aparece **7 vezes**, não 6, e o primeiro caractere não repetido é `'b'`, não `'i'` — `'b'` aparece uma única vez e antes de `'i'` no texto. Com `'e'` aparecendo só 2 vezes, o Top 3 real é `a` (7), seguido de um empate de 3 ocorrências entre `t`, `r` e `o`. O código resolve esse empate por ordem de primeira aparição no texto, resultando em `a` (7), `t` (3), `r` (3) — critério documentado em código e coberto por teste.

```
Entrada: "A Bateria do computador está Fraca!"
Texto higienizado: "abateriadocomputadorestafraca"
Primeiro caractere não repetido: 'b'
Top 3 caracteres mais frequentes:
  Letra 'a': 7 vezes
  Letra 't': 3 vezes
  Letra 'r': 3 vezes
```

### Questão 4 — Processamento financeiro

**Onde está:** `Question4/PaymentCalculator.cs` (cálculo) e `Question4/PaymentResult.cs` (resultado).

`PaymentCalculator.Calculate` compara a data de pagamento com a data de vencimento usando `DayNumber` (evitando problemas de fuso horário do `DateTime`) e aplica uma de três regras: desconto progressivo limitado a 10%, valor cheio na data, ou multa fixa de 2% mais juros simples de 0,5% ao dia. Os juros são sempre calculados sobre o valor base — nunca sobre a multa — porque a regra do enunciado especifica juros simples, não compostos.

**Decisões de implementação:**
 
- **Comparação de datas via `DayNumber`.** Uso `DateOnly` em vez de `DateTime` para representar datas de pagamento e vencimento, porque a regra de negócio é sobre **dias**, não sobre horários. Calcular `paymentDate.DayNumber - dueDate.DayNumber` dá diretamente a diferença em dias inteiros, sem o risco de erro de um dia que aconteceria comparando `DateTime` com horários diferentes ou fusos horários distintos.
- **`switch` de padrão sobre o sinal da diferença** (`< 0`, `> 0`, `_`) para escolher entre desconto, multa/juros ou valor cheio — deixa explícito que as três regras são mutuamente exclusivas.
- **Desconto limitado com `Math.Min`:** `Math.Min(diasDeAntecipacao * 0.01m, 0.10m)` aplica 1% ao dia, mas nunca ultrapassa o teto de 10%, sem precisar de um `if` separado para o caso de estourar o limite.
- **Juros sempre sobre o valor base, nunca sobre a multa.** O enunciado especifica "juros simples", que por definição incidem sempre sobre o principal — diferente de juros compostos, que incidiriam sobre saldo já acrescido de multa. Isso está isolado no método `CalculateLatePayment`, que calcula `Fine` e `Interest` a partir do mesmo `baseAmount`, nunca um a partir do outro.
- **Arredondamento centralizado em `RoundToCents`**, usando `MidpointRounding.AwayFromZero` (arredondamento comercial padrão, o mesmo usado por bancos), para que desconto, multa e juros sejam sempre arredondados da mesma forma, evitando que a soma final destoe de centavos por causa de arredondamentos inconsistentes.
- **`PaymentResult` é um `record`** com uma propriedade calculada (`FinalAmount`), em vez de já vir com o valor final pronto — isso deixa explícito, no retorno do método, exatamente quais componentes (desconto, multa, juros) compuseram o resultado, atendendo ao requisito do enunciado de "exibir o detalhamento".

| Data do pagamento | Situação | Valor final |
|---|---|---|
| 20/09/2026 | 20 dias de antecipação (desconto limitado a 10%) | R$ 900,00 |
| 05/10/2026 | 5 dias de antecipação (5% de desconto) | R$ 950,00 |
| 10/10/2026 | Na data | R$ 1.000,00 |
| 15/10/2026 | 5 dias de atraso (multa de 2% + juros de 2,5%) | R$ 1.045,00 |

---

## Parte 3: Questão 5 — API de Gerenciamento de Tarefas

### a) Como organizei as pastas/projetos

Uma solution com quatro projetos em camadas, seguindo a Clean Architecture:

```
src/
├── HavanTestTech.Domain/          # Entidade TodoItem, enum TodoStatus, DomainException
├── HavanTestTech.Application/     # Handlers (casos de uso), ITodoRepository, contratos
├── HavanTestTech.Infrastructure/  # InMemoryTodoRepository
└── HavanTestTech.Api/             # Minimal API, endpoints, tratamento de exceções
```

As dependências apontam sempre para dentro: `Api → Application → Domain`, e `Infrastructure → Application`. O `Domain` não referencia nenhum outro projeto da solution.

### b) Por que escolhi essa estrutura

- **Regras de negócio no domínio.** A validação de título (mínimo 5 caracteres) e o bloqueio de alterar status de uma tarefa já concluída ficam dentro de `TodoItem`. A entidade nunca existe em um estado inválido, não importa quem a está usando — API, um teste, ou futuramente um console.
- **Um handler por caso de uso**, em vez de um único service com vários métodos. `CreateTodoHandler`, `ChangeTodoStatusHandler`, `GetActiveTodosHandler` etc. têm responsabilidade única, o que facilita testar e adicionar uma nova operação sem alterar as existentes.
- **Persistência substituível.** O `Application` depende só da abstração `ITodoRepository`. Trocar a lista em memória por Entity Framework Core ou um banco externo muda apenas o `Infrastructure`, sem tocar em regra de negócio nenhuma.
- **Tempo controlável via `TimeProvider`.** Em vez de chamar `DateTime.UtcNow` diretamente, os handlers recebem um `TimeProvider` injetado. Isso permite testar `CompletedAt` com um valor previsível (`TestClock`, usado nos testes), sem depender do relógio real da máquina.
- **API fina.** Os endpoints só traduzem HTTP em chamadas aos handlers. Um único `ApiExceptionHandler` centraliza a conversão de exceções em respostas HTTP (400 para regra de negócio violada, 404 para tarefa não encontrada), no formato padrão RFC 7807 (Problem Details), evitando `try/catch` espalhado pelos endpoints.

### Como cada requisito funcional foi atendido
 
**1. Cadastro de tarefa.** `TodoItem` é construído com `Id` (`Guid`, gerado internamente — nunca recebido de fora, para não permitir que um cliente escolha o identificador de outra tarefa), `Title`, `Description`, `CreatedAt` (recebido via `TimeProvider`, não `DateTime.UtcNow` direto) e `Status` (inicia sempre como `Pending`). `CompletedAt` é `DateTime?`, nulo até a tarefa ser concluída — exatamente como pedido.
 
**2. Atualização de status.** `TodoItem.ChangeStatus(newStatus, changedAt)` já cuida de preencher `CompletedAt` automaticamente quando o novo status é `Completed`:
 
```csharp
public void ChangeStatus(TodoStatus newStatus, DateTime changedAt)
{
    if (Status == TodoStatus.Completed)
    {
        throw new DomainException("A completed task cannot have its status changed.");
    }
 
    Status = newStatus;
 
    if (newStatus == TodoStatus.Completed)
    {
        CompletedAt = changedAt;
    }
}
```
 
**3. Filtros e consultas.** `GetActiveTodosHandler` filtra por `item.IsActive` (uma propriedade calculada em `TodoItem`, `Status != TodoStatus.Completed`, para não duplicar essa lógica em todo lugar que precisar saber se uma tarefa está ativa). `GetCompletedTodosHandler` recebe duas datas (`DateOnly`) e converte para um intervalo `[inicio, fim + 1 dia)` sobre `CompletedAt` — o `+1 dia` no limite superior garante que o dia final do intervalo seja **inclusivo**, tratando corretamente o caso em que a tarefa foi concluída em qualquer horário daquele último dia.
 
**4. Validações de negócio.** Título vazio ou menor que 5 caracteres lança `DomainException` direto no construtor de `TodoItem` — a entidade nunca chega a existir em estado inválido. Alterar status de tarefa já concluída lança `DomainException` em `ChangeStatus` (mostrado acima). As duas regras têm teste unitário dedicado em `TodoItemTests`.
 
**5. Tratamento de exceções e retornos HTTP.** Como optei por Web API, o requisito "tratamento adequado de exceções e retornos HTTP" foi resolvido com `ApiExceptionHandler`, implementando `IExceptionHandler` (recurso nativo do .NET 8), que mapeia cada tipo de exceção para um código HTTP e devolve a resposta no formato padrão `ProblemDetails` (RFC 7807):
 
| Exceção | Status HTTP | Quando acontece |
|---|---|---|
| `DomainException` | 400 | Regra de negócio violada (título curto, status já concluído) |
| `InvalidRequestException` | 400 | Requisição malformada (enum de status fora do intervalo válido, intervalo de datas invertido) |
| `TodoNotFoundException` | 404 | `Id` não encontrado no repositório |
| Qualquer outra | 500 | Erro inesperado — a mensagem interna não é exposta ao cliente, só logada no servidor |
 
**6. Persistência in-memory.** `InMemoryTodoRepository` usa `ConcurrentDictionary<Guid, TodoItem>` em vez de uma `List<TodoItem>` simples, porque o repositório é registrado como singleton (`AddSingleton` em `DependencyInjection.cs`) e, portanto, compartilhado entre requisições HTTP simultâneas — uma coleção não thread-safe causaria condição de corrida sob concorrência real.
 
### Documentação interativa (Swagger)
 
A API expõe Swagger/OpenAPI em `/swagger`, habilitado apenas em ambiente de `Development`. Os cinco endpoints estão nomeados (`.WithName(...)`) e descritos (`.WithSummary(...)`), permitindo testar cada rota diretamente pelo navegador, sem precisar de `curl` ou Postman.


**Trade-off consciente:** para uma aplicação deste tamanho, um único projeto com pastas já seria suficiente. A separação em quatro projetos existe para que a regra de dependência seja **verificada pelo compilador** (o `Domain` fisicamente não consegue referenciar `Infrastructure`, por exemplo), e para deixar explícito, mesmo em um teste técnico pequeno, o entendimento de Clean Architecture e SOLID que a vaga exige.

### Endpoints

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/todos` | Cria uma tarefa (`201`, ou `400` se o título for inválido) |
| `GET` | `/api/todos` | Lista todas as tarefas |
| `GET` | `/api/todos/active` | Lista tarefas Pendentes e Em Andamento |
| `GET` | `/api/todos/completed?from=AAAA-MM-DD&to=AAAA-MM-DD` | Lista tarefas concluídas no intervalo (datas inclusivas) |
| `PATCH` | `/api/todos/{id}/status` | Altera o status (`404` se não existir, `400` se já concluída) |

### Testes

`dotnet test` roda a suíte completa: regras de `TodoItem` (Domain), comportamento dos handlers usando o repositório real in-memory (Application), e os algoritmos das Questões 2, 3 e 4. Alguns pontos de destaque na estratégia de testes:
 
- **`TestClock`** é uma implementação de `TimeProvider` totalmente controlável pelo teste (`Advance(TimeSpan)`), o que permite simular cenários como "esta tarefa foi concluída 2 horas depois de criada" sem depender do relógio real da máquina — o que tornaria o teste não determinístico.
- **Testes de handler usam o `InMemoryTodoRepository` real**, não um mock — como o repositório é simples o suficiente e não tem dependências externas, testar contra a implementação real dá mais confiança do que testar contra um duble que poderia divergir do comportamento real.
- **Casos de erro são testados tanto quanto os de sucesso**: `ChangeStatus_WithUnknownId_ThrowsNotFound`, `ChangeStatus_WithUndefinedStatus_ThrowsInvalidRequest`, `GetCompleted_WithStartAfterEnd_ThrowsInvalidRequest` — garantindo que as validações de negócio realmente bloqueiam o que devem bloquear, não só que o caminho feliz funciona.

---

## Bônus: Frontend React

SPA em React + TypeScript (Vite) que consome a API: formulário de criação com exibição de erros de validação vindos do backend, listagem em cards com badge colorido por status, e um botão que avança o status da tarefa (Pendente → Em Andamento → Concluída), atualizando a lista automaticamente após cada operação.

**Decisões de implementação:**
- **`useTodos` centraliza estado e chamadas à API** em um hook próprio, separado dos componentes visuais — `App.tsx` só orquestra `TodoForm` e `TodoCard`, sem saber como os dados são buscados ou persistidos.
- **Erros da API aparecem na tela**, extraídos do campo `detail` do `ProblemDetails` retornado pelo backend (por exemplo, a mensagem de título muito curto), em vez de um erro genérico.
- **O botão de avançar status usa uma tabela de transição (`NEXT_STATUS`)**, que já reflete a mesma regra do backend (uma tarefa concluída não tem próximo estado) — evitando que a interface ofereça uma ação que o backend rejeitaria.

A pasta pode ser rodada de forma independente (`cd frontend && npm install && npm run dev`), desde que a API esteja no ar em `http://localhost:5080` — endereço configurável via variável de ambiente `VITE_API_URL`.


## Autor 🧑🏻‍💻

| Nome | GitHub | LinkedIn |
|---|---|---|
| Marcos Gustavo | [GitHub](https://github.com/Marcosgmp) | [LinkedIn](https://www.linkedin.com/in/marcos-mpereira) |
