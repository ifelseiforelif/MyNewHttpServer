using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace MyNewHttpServer;

internal class Server
{
    readonly string _HOST = "http://127.0.0.1:8080/";
    public async Task RunServer()
    {
        HttpListener server = new HttpListener();
        server.Prefixes.Add(_HOST);
        server.Start();
        Console.WriteLine($"Server has been started {_HOST}");
        while (true)
        {
            try
            {
                HttpListenerContext ctx = await server.GetContextAsync();
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse res = ctx.Response;
                if (req.HttpMethod == "GET")
                {
                    Console.WriteLine($"Request: {req.Url} {req.HttpMethod} {req.Url?.AbsolutePath}");
                    string? param = req.Url?.AbsolutePath;
                    if(param==null)
                    {
                        param = "/";
                    }
                    string page = GetPageName(param);
                    
                    string path = Path.Combine(AppContext.BaseDirectory, "wwwroot", "pages", page);
                    string html = await File.ReadAllTextAsync(path, req.ContentEncoding);
                    byte[] bytes = Encoding.UTF8.GetBytes(html);
                    res.ContentLength64 = bytes.Length;
                    res.ContentType = "text/html; charset=utf-8";
                    res.StatusCode = 200;
                    using (Stream stream = res.OutputStream)
                    {
                        stream.Write(bytes, 0, bytes.Length);
                    }
                }
                res.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    private string GetPageName(string param) // /contacts
    {
        string result = param switch
        {
            "/contacts" => "contacts.html",
            "/about" => "about.html",
            "/" => "index.html",
            _=>"notfound.html"
        };
        return result;
    }
}
