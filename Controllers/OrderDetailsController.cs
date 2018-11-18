using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rabe_Celina_HW6.DAL;
using Rabe_Celina_HW6.Models;

namespace Rabe_Celina_HW6.Controllers
{
    public class OrderDetails : Controller
    {
        private readonly AppDbContext _context;

        public OrderDetails(AppDbContext context)
        {
            _context = context;
        }



        // GET: OrderDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail == null)
            {
                return NotFound();
            }
            return View(orderDetail);
        }

        // POST: OrderDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(OrderDetail orderDetail)
        {
            //Find the related order detail in the database
            OrderDetail DbOrdDet = _context.OrderDetails
                                        .Include(r => r.Order)
                                        .Include(r => r.Product)
                                        //what does this mean
                                        .FirstOrDefault(r => r.OrderDetailID ==
                                                            orderDetail.OrderDetailID);

            if (ModelState.IsValid)
            {
                try
                {
                    //update the related fields
                    DbOrdDet.QuantityOrdered = orderDetail.QuantityOrdered;
                    DbOrdDet.ProductPrice = orderDetail.ProductPrice;
                    DbOrdDet.ExtendedPrice = DbOrdDet.QuantityOrdered * DbOrdDet.ProductPrice;
                    //update the database
                    _context.OrderDetails.Update(DbOrdDet);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderDetailExists(DbOrdDet.OrderDetailID))
                    {
                        return NotFound();
                    }

                    else
                    {
                        throw;
                    }
                }


                //return to the order details
                return RedirectToAction("Details", "Orders", new { id = DbOrdDet.Order.OrderID });
            }
            return View(orderDetail);
        }
    


        // GET: OrderDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails
                .FirstOrDefaultAsync(m => m.OrderDetailID == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        //I DONT GET WHAT THIS IS DOING AHSDHFJ
        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            OrderDetail DbOrdDet = _context.OrderDetails
                                .Include(r => r.Order)
                                        
                                 //what does this mean
                                 .FirstOrDefault(r => r.OrderDetailID == id);

            _context.OrderDetails.Remove(DbOrdDet);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Orders", new { id = DbOrdDet.Order.OrderID });
        }

        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetails.Any(e => e.OrderDetailID == id);
        }
    }
}
