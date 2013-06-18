# Hyde - build a static website using Razor and the command line

Lightweight command line over RazorEngine. Quick hour hack for a project I'm working on to put a very simple command line over RazorEngine to build a static website using Razor templates.

## Example

1. Copy Hyde.exe, RazorEngine.dll and System.Web.Razor.dll to the same folder.

1. Create one or more templates saved as .cshtml.
<pre>
@{
 Layout = &quot;layout.cshtml&quot;;
}
&lt;h2&gt;@Model.H2&lt;/h2&gt;
&lt;div&gt;@Model.IndexContent&lt;/div&gt;      
</pre>

1. Optionally create one or more layouts saved as .cshtml.
<pre>
&lt;!doctype html&gt;
&lt;html&gt;
&lt;head&gt;
    &lt;title&gt;@Model.Title&lt;/title&gt;
&lt;/head&gt;
&lt;body&gt;
    &lt;h1&gt;Layout.cshtml&lt;/h1&gt;
    @RenderBody()
&lt;/body&gt;
&lt;/html&gt;
</pre>

3. Open a command prompt and type:
<pre>
Hyde -source c:\path_to_cshtml
-layouts c:\path_to_layout_cshtml
-json "{Title:'Test Hyde', H2:'Test Heading', IndexContent:'Some Test content for index'}"
-output c:\path_to_output_static_site
</pre>

This will parse all the .cshtml in  **-source** c:\path_to_cshtml,

with the **-layout(s)** c:\path_to_layout_cshtml, 

using the model specified in **-json** {Title:'Test Hyde', H2:'Test Heading', IndexContent:'Some Test content for index'}",

which will **-output** the static html to c:\path_to_output_static_site

### Output
<pre>
&lt;!doctype html&gt;
&lt;html&gt;
&lt;head&gt;
    &lt;title&gt;Test Hyde&lt;/title&gt;
&lt;/head&gt;
&lt;body&gt;
    &lt;h1&gt;Layout.cshtml&lt;/h1&gt;    
    &lt;h2&gt;Test Heading&lt;/h2&gt;
    &lt;div&gt;
       Some Test content for index
    &lt;/div&gt;
&lt;/body&gt;
&lt;/html&gt;
</pre>



 
