#pragma checksum "C:\Users\edina\42\SCS\proiect\CacheSimulator\CacheSimulator\Views\DirectMappedCacheSimulation\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "50bd3baf1786e9516fb73725189d8947b60f1efd"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_DirectMappedCacheSimulation_Index), @"mvc.1.0.view", @"/Views/DirectMappedCacheSimulation/Index.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"50bd3baf1786e9516fb73725189d8947b60f1efd", @"/Views/DirectMappedCacheSimulation/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"23ac09be4bcfaa7f9829a01d1a134874eaae1f3b", @"/Views/_ViewImports.cshtml")]
    public class Views_DirectMappedCacheSimulation_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("name", "_NavigationBar", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("name", "_SimulationInputsSideMenu", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.PartialTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 1 "C:\Users\edina\42\SCS\proiect\CacheSimulator\CacheSimulator\Views\DirectMappedCacheSimulation\Index.cshtml"
  
    ViewBag.Title = "Direct Mapped Cache";
    ViewBag.Data = "hello";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("partial", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagOnly, "50bd3baf1786e9516fb73725189d8947b60f1efd3700", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.PartialTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper.Name = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("partial", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagOnly, "50bd3baf1786e9516fb73725189d8947b60f1efd4817", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.PartialTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper.Name = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"

<div id=""addressBreakdown"">
    <table border=""1"" width=""40%"">
        <tr align=""center"">
            <td>Tag</td>
            <td width=""40%"">Index</td>
        </tr>
        <tr id=""addressRow"" align=""center"">
            <td><h id=""tagValue""></h></td>
            <td><h id=""indexValue""></h></td>
        </tr>
    </table>
</div>


<div id=""cacheTableContainer"">
");
            WriteLiteral(@"    <table border=1 width=100%>
        <tr align=center>
            <td width=15%>Index</td>
            <td width=15%>Valid</td>
            <td>Tag</td>
            <td>Data</td>
        </tr>

        <tr id=""cacheRow-0"" align=center><td>0</td><td id=""valid-0"">0</td><td id=""tag-0""></td><td id=""data-0""></td></tr>
        <tr id=""cacheRow-1"" align=center><td>1</td><td id=""valid-1"">0</td><td id=""tag-1""></td><td id=""data-1""></td></tr>
        <tr id=""cacheRow-2"" align=center><td>2</td><td id=""valid-2"">0</td><td id=""tag-2""></td><td id=""data-2""></td></tr>
        <tr id=""cacheRow-3"" align=center><td>3</td><td id=""valid-3"">0</td><td id=""tag-3""></td><td id=""data-3""></td></tr>
    </table>
</div>


<div id=""memoryTableContainer"">
    <table border=""1"">
");
#nullable restore
#line 43 "C:\Users\edina\42\SCS\proiect\CacheSimulator\CacheSimulator\Views\DirectMappedCacheSimulation\Index.cshtml"
          
            var blockCount = 0;
            for (var row = 0; row < 128; row++)
            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                <tr align=\"center\">\r\n");
#nullable restore
#line 48 "C:\Users\edina\42\SCS\proiect\CacheSimulator\CacheSimulator\Views\DirectMappedCacheSimulation\Index.cshtml"
                     for (var column = 0; column < 8; column++)
                    {

#line default
#line hidden
#nullable disable
            WriteLiteral("                        <td");
            BeginWriteAttribute("id", " id=", 1605, "", 1632, 1);
#nullable restore
#line 50 "C:\Users\edina\42\SCS\proiect\CacheSimulator\CacheSimulator\Views\DirectMappedCacheSimulation\Index.cshtml"
WriteAttributeValue("", 1609, "memory-"+blockCount, 1609, 23, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(">B ");
#nullable restore
#line 50 "C:\Users\edina\42\SCS\proiect\CacheSimulator\CacheSimulator\Views\DirectMappedCacheSimulation\Index.cshtml"
                                                    Write(blockCount);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n");
#nullable restore
#line 51 "C:\Users\edina\42\SCS\proiect\CacheSimulator\CacheSimulator\Views\DirectMappedCacheSimulation\Index.cshtml"
                        blockCount++;
                    }

#line default
#line hidden
#nullable disable
            WriteLiteral("                </tr>\r\n");
#nullable restore
#line 54 "C:\Users\edina\42\SCS\proiect\CacheSimulator\CacheSimulator\Views\DirectMappedCacheSimulation\Index.cshtml"
            }
        

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n    </table>\r\n</div>\r\n\r\n");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
