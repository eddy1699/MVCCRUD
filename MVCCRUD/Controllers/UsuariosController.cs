using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCCRUD.Models;

namespace MVCCRUD.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly MvccrudContext _context;

        public UsuariosController(MvccrudContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
              return _context.Usuarios != null ? 
                          View(await _context.Usuarios.ToListAsync()) :
                          Problem("Entity set 'MvccrudContext.Usuarios'  is null.");
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {

            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult?> Create([Bind("Id,Nombre,Fecha,Clave")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usuario);

                byte[] zpl = Encoding.UTF8.GetBytes("^XA\r\n\r\n^FX Top section with logo, name and address.\r\n^CF0,60\r\n^FO50,50^GB100,100,100^FS\r\n^FO75,75^FR^GB100,100,100^FS\r\n^FO93,93^GB40,40,40^FS\r\n^FO220,50^FDInternational Ship, Inc.^FS\r\n^CF0,30\r\n^FO220,115^FD1000 Shipping Lane^FS\r\n^FO220,155^FDShelbyville TN 38102^FS\r\n^FO220,195^FDUnited States (USA)^FS\r\n^FO50,250^GB700,3,3^FS\r\n\r\n^FX Second section with recipient address and permit information.\r\n^CFA,30\r\n^FO50,300^FDJohn Doe^FS\r\n^FO50,340^FD100 Main Street^FS\r\n^FO50,380^FDSpringfield TN 39021^FS\r\n^FO50,420^FDUnited States (USA)^FS\r\n^CFA,15\r\n^FO600,300^GB150,150,3^FS\r\n^FO638,340^FDPermit^FS\r\n^FO638,390^FD123456^FS\r\n^FO50,500^GB700,3,3^FS\r\n\r\n^FX Third section with bar code.\r\n^BY5,2,270\r\n^FO100,550^BC^FD12345678^FS\r\n\r\n^FX Fourth section (the two boxes on the bottom).\r\n^FO50,900^GB700,250,3^FS\r\n^FO400,900^GB3,250,3^FS\r\n^CF0,40\r\n^FO100,960^FDCtr. X34B-1^FS\r\n^FO100,1010^FDREF1 F00B47^FS\r\n^FO100,1060^FDREF2 BL4H8^FS\r\n^CF0,190\r\n^FO470,955^FDCA^FS\r\n\r\n^XZ");

                // adjust print density (8dpmm), label width (4 inches), label height (6 inches), and label index (0) as necessary
                var request = (HttpWebRequest)WebRequest.Create("http://api.labelary.com/v1/printers/8dpmm/labels/4x6/0/");
                request.Method = "POST";
                request.Accept = "application/pdf"; // omit this line to get PNG images back
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = zpl.Length;

                var requestStream = request.GetRequestStream();
                requestStream.Write(zpl, 0, zpl.Length);
                requestStream.Close();

                try
                {
                    var response = (HttpWebResponse)request.GetResponse();
                    var responseStream = response.GetResponseStream();
                   
                    var fileStream = System.IO.File.Create("label"+ DateTime.Now.ToString("MM-dd-yyyy hhmmssfff") + ".pdf"); // change file name for PNG images
                    responseStream.CopyTo(fileStream);
                    responseStream.Close();
                    fileStream.Close();
                }
                catch (WebException e)
                {
                    Console.WriteLine("Error: {0}", e.Status);
                }
                await _context.SaveChangesAsync();

                //return RedirectToAction(nameof(Index));
                return Create();
            }
            //return View(usuario);
            return Create();
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Fecha,Clave")] Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Usuarios == null)
            {
                return Problem("Entity set 'MvccrudContext.Usuarios'  is null.");
            }
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
          return (_context.Usuarios?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
