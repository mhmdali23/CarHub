﻿using Core.Entities;
using Core.Entities.Consts;
using Core.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly CarContext _context;

        public ReservationRepository(CarContext context)
        {
            _context = context;
        }
        public async Task<bool> CreateReservationAsync(int CarId,int ClientId)
        {
            var reservation = new Reservation();   
            reservation.CarId = CarId;
            reservation.ClientId = ClientId;
            reservation.ReservationFee =  _context.Cars.SingleOrDefault(s => s.Id == CarId).Price * CarServicesPrices.ReservationRatio;
            await _context.Reservations.AddAsync(reservation);
            
            return await SaveAsync();
        }

        public async Task<IReadOnlyList<Reservation>> GetReservationsAsync()
        {
            return await _context.Reservations.Include(r => r.Client).Include(r => r.Car).Include(c => c.Car.Brand)
                .Include(c => c.Car.Make)
                .Include(c => c.Car.Model).AsNoTracking().ToListAsync();
        }

        public  async Task<bool> SaveAsync()
        {
            var saved = await _context.SaveChangesAsync();
            return saved >0? true:false;
        }


        public async Task<IReadOnlyList<Client>> GetClientsToAlert()
        {
            var clients = await _context.Clients
                                  .Include(c => c.Reservations)
                                  .ThenInclude(r => r.Car)                             
                                  .Where(
                                  c => c.Reservations.OrderByDescending(c => c.EndDate).First().EndDate.Day == DateTime.Today.AddDays(1).Day
                                   )
                                  .ToListAsync();

            return clients;


        }

        public async Task<Reservation?> GetReservationByIdAsync(int id)
        {
            return await _context.Reservations
                                 .Where(r => r .Id == id)
                                 .Include(r => r.Car)
                                 .Include(r => r.Car.Brand)
                                 .Include(r => r.Car.Model)
                                 .FirstOrDefaultAsync();
        }

        public async Task<bool> EndReservationAsync(Reservation reservation)
        {
            

            reservation.EndDate = DateTime.Today;

            return await SaveAsync();
        }

        public Task<bool> CancelReservationAsync(Reservation reservation)
        {
            _context.Reservations.Remove(reservation);

            return SaveAsync();
        }
    }
}
