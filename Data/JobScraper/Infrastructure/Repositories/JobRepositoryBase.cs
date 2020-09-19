﻿using AutoMapper;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public abstract class JobRepositoryBase<T> : ReadRepositoryBase<T>
		where T: JobUrl
    {
		private readonly DataContext _context;

		private DbSet<T> _entities;

		public JobRepositoryBase(DataContext context, IMapper mapper) : base(context, mapper)
		{
			_context = context;

			_entities = _context.Set<T>();
		}

		public void Insert(T entity)
		{
			var exists = _context.JobUrls.Any(
				j => j.Title == entity.Title
			&& j.Company == entity.Company
			&& j.Salary == entity.Salary);

			if (!exists)
			{
				_context.JobUrls.Add(entity);
			}

			_context.SaveChanges();
		}

		public void InsertRange(IEnumerable<T> entities)
		{
			foreach (var entity in entities)
			{
				Insert(entity);
			}
		}

		public void UpdateSalary(int id, int? min, int? max)
		{
			var jobEntity = FindById(id);
			jobEntity.SalaryMin = min;
			jobEntity.SalaryMax = max;

			_context.SaveChanges();
		}
	}
}