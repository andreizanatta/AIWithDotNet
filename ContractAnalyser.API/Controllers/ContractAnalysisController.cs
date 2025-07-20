using Microsoft.AspNetCore.Mvc;

namespace ContractAnalyser.API.Controllers
{
    [ApiController]
    [Route("api/contract-analysis")]
    public class ContractAnalysisController :ControllerBase
    {
        private HttpClient _client;
        public ContractAnalysisController(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("OpenAI");
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> Analyze(ContractInput input)
        {
            var body = new
            {
                model = "gpt-4o",
                input = $"Analise o contrato abaixo:\n\n{input.ContractText}\n\nAnalise e identifique possíveis cláusulas problemáticas ou pontos que possam gerar desequilíbrio para o freelancer. Apresente a análise em tópicos.",
                instructions = "Você é um advogado experiente em contratos de tecnologia.\r\n\r\nSua função é analisar textos contratuais e identificar cláusulas que possam ser abusivas, desproporcionais ou prejudiciais para o prestador de serviço (freelancer).\r\n\r\nSempre que for solicitado, você deve:\r\n- Destacar cláusulas potencialmente problemáticas,\r\n- Explicar por que podem ser prejudiciais ao freelancer,\r\n- Sugerir alternativas mais justas ou equilibradas,\r\n- Usar uma linguagem clara, objetiva e profissional, mesmo para não juristas.\r\n\r\nPriorize temas como: propriedade intelectual, exclusividade, prazos, pagamento, rescisão, multas, portfólio e confidencialidade.\r\n\r\nApresente de forma resumida em tópicos.\r\n",
                temperature = 0.6,
                max_output_tokens = 1000
            };

            var response = await _client.PostAsJsonAsync("responses", body);

            //var result = await response.Content.ReadAsStringAsync();
            var result = await response.Content.ReadFromJsonAsync<OpenAiResponse>();
            var text = result?.OutPut?.First().Content?.First().Text;
            return Ok(text);
        }
    }

    public class ContractInput
    {
        public string ContractText { get; set; }
    }

    public class OpenAiResponse
    {
        public List<OutPut> OutPut { get; set; }
    }

    public class OutPut
    {
        public List<Content> Content { get; set; }
    }

    public class Content
    {
        public string Text { get; set; }
        public string Type { get; set; }
    }
}
