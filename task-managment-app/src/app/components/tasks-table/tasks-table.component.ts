import { Component, OnInit } from '@angular/core';
import { TaskDataService } from 'src/app/services/task-data.service';
import { Task, Priority, Status } from 'src/app/models/task';
import { filter } from 'rxjs/operators';
import { Subscription } from 'rxjs';
@Component({
  selector: 'app-tasks-table',
  templateUrl: './tasks-table.component.html',
  styleUrls: ['./tasks-table.component.css']
})
export class TasksTableComponent implements OnInit {
  public tasks:Task[]=[];
  public tasksSubjectSubscription:Subscription;
  public getTasksSubscription:Subscription;
  public deleteTaskSubscription:Subscription;
  public closeTaskSubscription:Subscription;
  public selectTaskSubscription:Subscription;

  get Priority(){ return Priority;} 
  get Status(){ return Status;} 
  constructor(private dataService :TaskDataService) { 
  
  this.tasksSubjectSubscription =  this.dataService.TasksSubject.pipe(filter(data=> !!data)).subscribe(
      (data)=>{this.tasks=data;},
      (err:any)=>{}
    );
  }

  ngOnInit() {

   this.getTasksSubscription = this.dataService.getTaskList().pipe(filter(data=> !!data)).subscribe();

  }
  ngOnDestroy() {
    this.tasksSubjectSubscription.unsubscribe();
    this.getTasksSubscription.unsubscribe();
    this.deleteTaskSubscription.unsubscribe();
    this.closeTaskSubscription.unsubscribe();
    this.selectTaskSubscription.unsubscribe();
  }

  onDelete(taskId:number){
    this.deleteTaskSubscription = this.dataService.deleteTask(taskId).pipe(filter(data=> !!data)).subscribe();
  }
  onChangeStatus(taskId:number){
    this.closeTaskSubscription = this.dataService.closeTask(taskId).pipe(filter(data=> !!data)).subscribe();
  }
  onEdit(taskId:number){
    this.selectTaskSubscription = this.dataService.selectTask(taskId).pipe(filter(data=> !!data)).subscribe();
  }
}
