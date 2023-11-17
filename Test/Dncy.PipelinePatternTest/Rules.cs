using Newtonsoft.Json;
using RulesEngine.Models;
using System.Text.Json;

namespace Dncy.PipelinePatternTest;

public class Rules
{
    private const string rules = """
        [
          {
            "WorkflowName": "serveAlert",
            "Rules": [
              {
                "RuleName": "cpu80",
                "SuccessEvent": "80",
                "ErrorMessage": "cpu 使用率正常.",
                "ErrorType": "Error",
                "RuleExpressionType": "LambdaExpression",
                "Expression": "Level <= 80"
              }
            ]
          }
        ]
        """;
    public RulesEngine.RulesEngine rulesEngine()
    {
        var sdsd=JsonConvert.DeserializeObject<Workflow[]>(rules);
        return new RulesEngine.RulesEngine(sdsd);
    }

}