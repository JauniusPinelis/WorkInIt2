﻿using Application.Interfaces;
using Application.Services;
using AutoMapper;
using Domain.Helpers;
using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DataServices
{
    public abstract class DataServiceBase<T> where T : JobUrl
    {
        private readonly IScrapeService _scrapeService;
        private readonly CompanyService _companyService;
        private readonly IRepository<T> _repository;

        private readonly IMapper _mapper;

        public DataServiceBase(IScrapeService scrapeService, IRepository<T> repository,
            CompanyService companyService, IMapper mapper)
        {
            _repository = repository;
            _scrapeService = scrapeService;
            _companyService = companyService;

            _mapper = mapper;
        }

        public void ScrapeJobs()
        {
            if (_repository.GetAll().Any())
            {
                var urlDtos = _scrapeService.ScrapeUrls(2);
                var urls = _mapper.Map<IEnumerable<T>>(urlDtos);
                _repository.InsertRange(urls);
            }
            else
            {
                var urlDtos = _scrapeService.ScrapeUrls();
                var urls = _mapper.Map<IEnumerable<T>>(urlDtos);
                _repository.InsertRange(urls);
            }
        }

        public void ScrapeHtmls()
        {
            var jobs = _repository.GetAll()
                .Where(j => !String.IsNullOrEmpty(j.Url) && String.IsNullOrEmpty(j.Html)).ToList();

            foreach (var job in jobs)
            {
                var html = _scrapeService.ScrapeInfo(job.Url);

                _repository.UpdateHtml(job.Id, html);
            }
        }

        public void ProcessCompanies()
        {
            var jobsWithNoCompanies = _repository.GetAll().Where(j => !j.CompanyId.HasValue
            && !String.IsNullOrEmpty(j.CompanyName)).ToList();

            foreach (var job in jobsWithNoCompanies)
            {
                if (!_companyService.DoesContain(job.CompanyName))
                {
                    var companyId = _companyService.Insert(job.CompanyName, job.LogoUrl);
                    _repository.UpdateCompany(job.Id, companyId);


                }
                else
                {
                    var company = _companyService.GetByName(job.CompanyName);
                    if (company != null)
                    {
                        _repository.UpdateCompany(job.Id, company.Id);
                    }
                }
            }
        }

        public void ProcessCompanyLogos()
        {
            var companiesWithNoLogos = _companyService.GetAll()
                .Where(c => c.ImageData == null
                && !String.IsNullOrEmpty(c.Logourl)).ToList();

            foreach (var company in companiesWithNoLogos)
            {
                Task.Delay(1000); //Maybe create a service for this delayer?

                var image = _scrapeService.ScrapeLogo(company.Logourl);

                if (image != null)
                {
                    _companyService.UpdateImage(company.Id, image, company.Logourl);

                }
            }
        }

        public void ProcessTags()
        {
            var jobsWithNoTags = _repository.GetAll()
                .Where(j => !j.Tags.Any() && !String.IsNullOrEmpty(j.Html)).ToList();

            var tags = _repository.GetAllTags().ToList();

            foreach (var job in jobsWithNoTags)
            {
                var html = job.Html;
                var extractedTags = TagHelpers.ExtractTags(html, tags);

                _repository.UpdateTags(job.Id, extractedTags);
            }


        }
    }
}
