import { Component, OnInit } from '@angular/core';
import { JobService } from 'src/app/services/job.service';

@Component({
  selector: 'job-table',
  templateUrl: './jobtable.component.html',
  styleUrls: ['./jobtable.component.scss']
})
export class JobtableComponent implements OnInit {

  jobs: any;

  constructor(private jobService: JobService) { }

  ngOnInit(): void {
    this.retrieveJobs();
  }

  retrieveJobs(): void {
    this.jobService.getAll().subscribe(
      data => {
        this.jobs = data;
        console.log(data);
      },
      error => {
        console.log(error);
      }
    )
  }

}