/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using GuiStack.Extensions;
using GuiStack.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GuiStack.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ProtoController : Controller
    {
        private static readonly Regex FileNameSanitizer = new Regex(@"[^a-z0-9_.+\-=\[\](){}!#$%&]", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        private static readonly Regex ProtoImportParser = new Regex("^[\\t ]*import[\\t ]+\"([^\"\\r\\n]+?)\";", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline);

        public ProtoController()
        {
        }

        private ActionResult HandleException(Exception ex)
        {
            if(ex == null)
                throw new ArgumentNullException(nameof(ex));

            Console.Error.WriteLine(ex);
            return StatusCode((int)HttpStatusCode.InternalServerError, new { error = ex.Message });
        }

        [HttpGet, HttpPost]
        public ActionResult ClearSession()
        {
            try
            {
                if(!HttpContext.Session.Keys.Contains("proto-identifier"))
                    return StatusCode((int)HttpStatusCode.BadRequest, new { error = "No protobuf identifier is present in the current session" });

                string identifier = HttpContext.Session.GetString("proto-identifier");
                string protoDir = Path.Combine(Path.GetTempPath(), $"guistack-proto-{identifier}");

                Directory.Delete(protoDir, true);

                return Ok();
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{filename}")]
        public ActionResult Download([FromRoute] string filename)
        {
            if(!HttpContext.Session.Keys.Contains("proto-identifier"))
                return StatusCode((int)HttpStatusCode.BadRequest, new { error = "No protobuf identifier is present in the current session" });

            filename = FileNameSanitizer.Replace(Path.GetFileName(filename.DecodeRouteParameter()), "");

            string identifier = HttpContext.Session.GetString("proto-identifier");
            string protoDir = Path.Combine(Path.GetTempPath(), $"guistack-proto-{identifier}");
            string protoPath = Path.Combine(protoDir, filename);

            try
            {
                var fileStream = new FileStream(protoPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(fileStream, "application/octet-stream");
            }
            catch(FileNotFoundException ex)
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }

        const long MaxFileSize = 512 * 1024L; // 512 KB

        [HttpPost]
        [Produces("application/json")]
        [RequestSizeLimit(MaxFileSize)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxFileSize)]
        public async Task<ActionResult> Upload(List<IFormFile> files)
        {
            string identifier = DateTime.Now.ToString("yyyyMMddHHmmssffffff");
            string protoDir = Path.Combine(Path.GetTempPath(), $"guistack-proto-{identifier}");

            List<string> protofiles = new List<string>();

            try
            {
                Directory.CreateDirectory(protoDir);

                foreach(var file in files)
                {
                    string filename = FileNameSanitizer.Replace(Path.GetFileName(file.FileName), "");
                    string filePath = Path.Combine(protoDir, filename);

                    if(file.Length > MaxFileSize)
                        throw new Exception($"File \"{filename}\" exceeds the maximum allowed size of {MaxFileSize.ToFormattedFileSize()}");

                    using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                    await file.CopyToAsync(fileStream);

                    protofiles.Add(filename);
                }

                HttpContext.Session.SetString("proto-identifier", identifier);

                return Json(protofiles);
            }
            catch(Exception ex)
            {
                try { Directory.Delete(protoDir, true); } catch { }
                return HandleException(ex);
            }
        }

        // 
        // Unused
        // 
        [HttpPost]
        [Produces("application/json")]
        [RequestSizeLimit(MaxFileSize)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxFileSize)]
        public async Task<ActionResult> Generate(List<IFormFile> files)
        {
            string identifier = DateTime.Now.ToString("yyyyMMddHHmmssffffff");
            string protoDir = Path.Combine(Path.GetTempPath(), $"guistack-proto-{identifier}");

            try
            {
                if(string.IsNullOrWhiteSpace(EnvironmentVariables.ProtobufCompilerExecutable))
                    throw new Exception("Environment variable PROTOC_EXECUTABLE not specified");

                if(!System.IO.File.Exists(EnvironmentVariables.ProtobufCompilerExecutable))
                    throw new FileNotFoundException($"Protoc compiler not found at \"{EnvironmentVariables.ProtobufCompilerExecutable}\"", EnvironmentVariables.ProtobufCompilerExecutable);

                Directory.CreateDirectory(protoDir);

                HashSet<string> protofiles = new HashSet<string>();

                foreach(var file in files)
                {
                    string filename = FileNameSanitizer.Replace(Path.GetFileName(file.FileName), "");
                    string filePath = Path.Combine(protoDir, filename);

                    if(file.Length > MaxFileSize)
                        throw new Exception($"File \"{filename}\" exceeds the maximum allowed size of {MaxFileSize.ToFormattedFileSize()}");

                    using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                    await file.CopyToAsync(fileStream);

                    protofiles.Add(filename);
                }

                foreach(string filename in protofiles.ToArray())
                {
                    string filePath = Path.Combine(protoDir, filename);
                    string contents = await System.IO.File.ReadAllTextAsync(filePath);

                    foreach(Match match in ProtoImportParser.Matches(contents))
                    {
                        string importedFile = match.Groups[1].Value;
                        protofiles.Remove(importedFile);
                    }
                }

                List<string> result = new List<string>();

                foreach(string filename in protofiles)
                {
                    string filePath = Path.Combine(protoDir, filename);
                    string outputFile = Path.Combine(protoDir, $"{filename}.bin");

                    StringBuilder stdout = new StringBuilder();
                    StringBuilder stderr = new StringBuilder();

                    ProcessStartInfo psi = new ProcessStartInfo(EnvironmentVariables.ProtobufCompilerExecutable, $"\"-I{protoDir}\" \"{filePath}\" \"-o{outputFile}\"");
                    psi.ErrorDialog = false;
                    psi.CreateNoWindow = true;
                    psi.WindowStyle = ProcessWindowStyle.Hidden;
                    psi.WorkingDirectory = protoDir;
                    psi.UseShellExecute = false;
                    psi.RedirectStandardError = true;
                    psi.RedirectStandardOutput = true;

                    using Process protoc = new Process();
                    protoc.StartInfo = psi;
                    protoc.EnableRaisingEvents = true;

                    protoc.OutputDataReceived += (sender, e) => stdout.Append(e.Data);
                    protoc.ErrorDataReceived += (sender, e) => stderr.Append(e.Data);

                    protoc.Start();
                    protoc.BeginOutputReadLine();
                    protoc.BeginErrorReadLine();
                    protoc.WaitForExit(10000);

                    if(!protoc.HasExited)
                    {
                        protoc.Kill();
                        throw new Exception($"Failed to compile \"{filename}\": Operation timed out");
                    }

                    if(protoc.ExitCode != 0)
                    {
                        string errorMessage = $"Failed to compile \"{filename}\": Protobuf compiler exited with code {protoc.ExitCode}";

                        if(stderr.Length > 0)
                        {
                            errorMessage += $"\n\nError log:\n{stderr}";
                        }

                        if(stdout.Length > 0)
                        {
                            errorMessage += $"\n\nOutput log:\n{stdout}";
                        }

                        throw new Exception(errorMessage);
                    }

                    if(!System.IO.File.Exists(outputFile))
                        throw new FileNotFoundException($"Failed to compile \"{filename}\": Process exited successfully, but the output file was not found?");

                    FileDescriptorSet definition = FileDescriptorSet.Parser.ParseFrom(System.IO.File.ReadAllBytes(outputFile));

                    var file = definition.File.FirstOrDefault();
                    if(file == null)
                        continue;

                    var protoJson = JsonFormatter.Default.Format(file);
                    result.Add(protoJson);
                }

                HttpContext.Session.SetString("proto-identifier", identifier);

                return Content("[" + string.Join(", ", result) + "]", "application/json");
            }
            catch(Exception ex)
            {
                try { Directory.Delete(protoDir, true); } catch { }
                return HandleException(ex);
            }
        }
    }
}
