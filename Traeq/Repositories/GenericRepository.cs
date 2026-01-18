using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Traeq.Data;
using Traeq.Models;
using Traeq.Repositories;

namespace Traeq.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly AppDbContext _db;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext db)
        {
            _db = db;
            _dbSet = _db.Set<T>();
        }

        public void Add(T entity)
        {
           
            var isSeeded = entity.CreateDate.HasValue;

            if (!isSeeded)
            {
                entity.IsActive = true;
                entity.IsDelete = false;
                entity.CreateDate = DateTime.Now;
            }

            entity.EditDate = DateTime.Now;

            _dbSet.Add(entity);
        }

        public void Update(int id, T entity)
        {
            var data = Find(id);
            if (data == null) return;

            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                if (property.Name == "Id" ||
                    property.Name == "CreateId" ||
                    property.Name == "CreateDate" ||
                    property.Name == "IsActive")
                    continue;

                var newValue = property.GetValue(entity);
                if (newValue != null)
                {
                    property.SetValue(data, newValue);
                }
            }

            data.EditDate = DateTime.Now;

            var incomingEditId = typeof(T).GetProperty("EditId")?.GetValue(entity) as string;
            data.EditId = !string.IsNullOrWhiteSpace(incomingEditId) ? incomingEditId : Guid.NewGuid().ToString();

            _db.SaveChanges();
        }

        public void Delete(int id, T entity)
        {
            var data = Find(id);
            data.IsDelete = !data.IsDelete;
            data.EditDate = DateTime.Now;
            _dbSet.Update(data);
            _db.SaveChanges();
        }

        public void Active(int id)
        {
            var data = Find(id);
            data.IsActive = !data.IsActive;
            data.EditDate = DateTime.Now;
            _dbSet.Update(data);
            _db.SaveChanges();
        }
        
        public List<T> View()
        {
            return _dbSet.Where(x => x.IsDelete == false).ToList();
        }

        public List<T> ViewClient()
        {
            try
            {
                return _dbSet.Where(x => x.IsDelete == false && x.IsActive == true).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"ERROR IN ENTITY: {typeof(T).Name}", ex);
            }
        }

        public T Find(int id)
        {
            return _dbSet.Find(id);
        }
    }
}