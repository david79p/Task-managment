import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl} from '@angular/forms';
import { TaskDataService } from 'src/app/services/task-data.service';
import { filter } from 'rxjs/operators';
import { Task, Priority, Status } from 'src/app/models/task';
import { Subscription } from 'rxjs';
import * as moment from 'moment';
@Component({
  selector: 'app-task',
  templateUrl: './task.component.html',
  styleUrls: ['./task.component.css']
})
export class TaskComponent implements OnInit {
  public taskSubjectSubscription:Subscription;
  public saveTaskSubscription:Subscription;
  public taskFormGroup:FormGroup;

  constructor(private dataService :TaskDataService) { 
   
  }

  ngOnInit() {
    this.taskFormGroup = new FormGroup({
      id : new FormControl(null),
      name : new FormControl(null),
      todoDate: new FormControl(null),
      taskPriority: new FormControl(null),
      taskStatus: new FormControl(null)
    });
    this.taskSubjectSubscription = this.dataService.TaskSubject.pipe(filter(data=> !!data)).subscribe(
      (data:Task)=>{ const taskView = this.transformModelToView(data);  this.taskFormGroup.setValue(taskView)},
      (err:any)=>{});
  }
  ngOnDestroy() {
    this.taskSubjectSubscription.unsubscribe();
    this.saveTaskSubscription.unsubscribe();
  }
  onSave(){
    //The next line didnt put the right values on the object
    //const task:Task = <Task>this.taskFormGroup.getRawValue(); 
    const taskView =  this.taskFormGroup.getRawValue();
    const task = this.transformViewToModel(taskView);
    if (this.validateTaskModel(task)){
      this.saveTaskSubscription = this.dataService.saveTask(task).pipe(filter(data=> !!data)).subscribe();
    }
   
    
  }
  validateTaskModel(task:Task){
    let isvalid:boolean = true;
    if (task.id < 0){
       alert('Invalid task ID');
       isvalid = false;
    }
    else if (task.name==null || task.name==""){
      alert('Task name is empty');
       isvalid = false;
    }
    else if (task.taskPriority==null || task.taskPriority<=0 || task.taskPriority>=4){
      alert('Task priority has wrong value');
       isvalid = false;
    }
    else if (task.taskStatus==null || task.taskStatus<=0 || task.taskStatus>=3){
      alert('Task status has wrong value');
       isvalid = false;
    }
    else if (isNaN(task.todoDate.getDate())){
      alert('Task todo date has invalid date value');
      isvalid = false;
    }
    return isvalid;
  }
  transformModelToView(task : Task):any{
    return {id:task.id,name:task.name,todoDate:moment(task.todoDate).format('DD/MM/YYYY'),taskPriority:Priority[task.taskPriority],taskStatus:Status[task.taskStatus]};
  }
  transformViewToModel(taskView : any):Task{
    let task:Task = new Task();
    task.id = taskView.id==null?0:parseInt(taskView.id);
    task.name = taskView.name;
    task.taskPriority = Priority[<string>taskView.taskPriority];
    task.todoDate =  moment(<string>taskView.todoDate,'DD/MM/YYYY').toDate();
    task.taskStatus =  taskView.id==null?1:Status[<string>taskView.taskStatus];
    return task;
  }
}
