using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;

namespace AIWithDotNet.Controllers
{
    [Route("api/contract-analysis")]
    [ApiController]
    public class ContractAnalysisController : ControllerBase
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
                input = $"Analise o texto abaixo:\r\n{input.ContractText}\r\n",
                instructions = "Você é um especialista jurídico com foco em contratos de prestação de serviços para freelancers.\r\n\r\nSua função é analisar textos contratuais e identificar cláusulas que possam ser abusivas, desproporcionais ou prejudiciais para o prestador de serviço (freelancer).\r\n\r\nSempre que for solicitado, você deve:\r\n- Destacar cláusulas potencialmente problemáticas,\r\n- Explicar por que podem ser prejudiciais ao freelancer,\r\n- Sugerir alternativas mais justas ou equilibradas,\r\n- Usar uma linguagem clara, objetiva e profissional, mesmo para não juristas.\r\n\r\nPriorize temas como: propriedade intelectual, exclusividade, prazos, pagamento, rescisão, multas, portfólio e confidencialidade.\r\n",
                temperature = 0.6,
                max_output_tokens = 1000
            };

            var response = await _client.PostAsJsonAsync("responses", body);

            var result = await response.Content.ReadAsStringAsync();

            return Ok(result);
        }
    }

    public class ContractInput
    {
        public string ContractText { get; set; }
    }
}
