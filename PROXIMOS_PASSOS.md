# Próximos Passos — Sistema de Automação de Feedback via WhatsApp

## Objetivo

Automatizar o envio de mensagens no WhatsApp para clientes com baixa satisfação (NPS), utilizando:

* Importação de planilhas Excel
* Banco PostgreSQL
* Backend .NET
* IA para geração de mensagens
* Evolution API para integração com WhatsApp

---

# Etapa 1 — Buscar números no banco de dados

## Objetivo

Utilizar os números já cadastrados no banco para enviar mensagens automaticamente.

---

## Fluxo esperado

```text
Excel -> Importação -> Banco -> Busca cliente -> Busca telefone -> Geração IA -> Envio WhatsApp
```

---

## Verificações necessárias

### 1. Confirmar se os clientes possuem telefone salvo

Tabela:

```text
Clientes
```

Campo:

```text
Telefone
```

---

### 2. Validar formato do telefone

Formato ideal:

```text
5538999999999
```

Formato inválido:

```text
(38) 99999-9999
```

---

## Criar função de limpeza de telefone

Exemplo:

```csharp
public static string LimparTelefone(string telefone)
{
    return new string(
        telefone.Where(char.IsDigit).ToArray()
    );
}
```

---

# Etapa 2 — Enviar mensagens automaticamente

## Objetivo

Enviar mensagens automaticamente após importar os feedbacks.

---

## Fluxo esperado

```text
Importação Excel
↓
Salvar Feedback
↓
Gerar Mensagem IA
↓
Enviar WhatsApp
```

---

## Implementação

Após salvar o feedback:

```csharp
await _evolutionService.SendMessage(
    cliente.Telefone,
    mensagemIA
);
```

---

# Etapa 3 — Gerar mensagens com IA

## Objetivo

Criar mensagens personalizadas com base:

* no nome do cliente
* na nota dada
* no contexto do feedback

---

## Regras

### Nota baixa (0-3)

Mensagem mais empática.

### Nota média (4-6)

Mensagem mais amigável.

### Nota alta (7-10)

Opcional:

* agradecer
* solicitar avaliação
* solicitar indicação

---

## Exemplo de prompt

```text
O cliente Frank deu nota 3.

Crie uma mensagem educada, curta e humanizada
pedindo desculpas pela experiência ruim e
perguntando como podemos melhorar.
```

---

# Etapa 4 — Criar fila de mensagens

## Objetivo

Evitar perda de mensagens em caso de:

* queda da API
* internet instável
* falha do WhatsApp
* grande volume de mensagens

---

## Fluxo correto

```text
Excel
↓
Banco
↓
Fila de Mensagens
↓
Worker
↓
WhatsApp
```

---

## Criar tabela de fila

```csharp
public class MessageQueue
{
    public int Id { get; set; }

    public string Numero { get; set; }

    public string Mensagem { get; set; }

    public bool Enviado { get; set; }

    public DateTime CriadoEm { get; set; }

    public DateTime? EnviadoEm { get; set; }
}
```

---

# Etapa 5 — Criar Worker de envio

## Objetivo

Enviar mensagens automaticamente em segundo plano.

---

## Funcionamento

A cada alguns segundos:

1. Busca mensagens pendentes
2. Envia via Evolution API
3. Marca como enviada

---

## Benefícios

* maior estabilidade
* controle de falhas
* reenvio automático
* escalabilidade

---

# Etapa 6 — Evitar mensagens duplicadas

## Problema

O mesmo Excel pode ser importado duas vezes.

Isso pode gerar múltiplos envios para o mesmo cliente.

---

## Solução

Antes de salvar:

```csharp
var existe = _context.Feedbacks.Any(...)
```

---

## Estratégia recomendada

Criar hash único:

```text
CNPJ + Data + Nota
```

---

# Etapa 7 — Melhorias futuras

## Dashboard

Criar painel com:

* quantidade de mensagens enviadas
* pendentes
* erros
* clientes respondidos

---

## Agendamento

Enviar mensagens em horários específicos.

---

## Webhook

Receber respostas do WhatsApp automaticamente.

---

## IA avançada

Treinar respostas mais naturais e personalizadas.

---

# Arquitetura Final

```text
Excel
 ↓
Importador
 ↓
PostgreSQL
 ↓
IA
 ↓
Fila
 ↓
Worker
 ↓
Evolution API
 ↓
WhatsApp
```

---

# Status Atual do Projeto

## Já implementado

* Evolution API funcionando
* WhatsApp conectado
* Backend .NET funcionando
* Controller de envio funcionando
* Integração HTTP funcionando
* Importação Excel funcionando
* Geração IA iniciada

---

## Próximos passos prioritários

1. Buscar números automaticamente do banco
2. Enviar mensagens automaticamente
3. Criar fila de mensagens
4. Evitar mensagens duplicadas
5. Criar worker de envio
