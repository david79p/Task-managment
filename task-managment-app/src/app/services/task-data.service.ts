import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { Task, Priority } from '../models/task';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { map } from 'rxjs/operators';
import { catchError } from 'rxjs/internal/operators/catchError';
@Injectable({
  providedIn: 'root'
})
export class TaskDataService {
  private _taskSubject : BehaviorSubject<Task> = new BehaviorSubject<Task>(null);
  private _tasksListSubject : BehaviorSubject<Task[]> = new BehaviorSubject<Task[]>(null);
  constructor(private httpClient:HttpClient) { }

  getTaskList():Observable<Task[]> {
    return this.httpClient.get(environment.task_api_url).pipe(map(response => {
      this._tasksListSubject.next(<Task[]>response); 
      return of(<Task[]>response);
    })
    , catchError(err => {      
       return of(null);
    })
  );
  }
  deleteTask(taskId):Observable<Task[]> {
    const deleteURI = `${environment.task_api_url}/${taskId}`
    return this.httpClient.delete(deleteURI).pipe(map(response => {
      this._tasksListSubject.next(<Task[]>response); 
      return of(<Task[]>response);
    })
    , catchError(err => {      
       return of(null);
    })
  );
  }
  closeTask(taskId):Observable<Task[]> {
    const patchURI = `${environment.task_api_url}/${taskId}`
    return this.httpClient.patch(patchURI,null).pipe(map(response => {
      this._tasksListSubject.next(<Task[]>response); 
      return of(<Task[]>response);
    })
    , catchError(err => {      
       return of(null);
    })
  );
  }
  selectTask(taskId):Observable<Task> {
    const getURI = `${environment.task_api_url}/${taskId}`
    return this.httpClient.get(getURI).pipe(map(response => {
      this._taskSubject.next(<Task>response); 
      return of(<Task>response);
    })
    , catchError(err => {      
       return of(null);
    })
  );
  }
  saveTask(task:Task):Observable<Task[]>{
    if (task.id==0){
      const postURI = `${environment.task_api_url}`
      return this.httpClient.post(postURI,task).pipe(map(response => {
        this._tasksListSubject.next(<Task[]>response); 
        return of(<Task>response);
      })
      , catchError(err => {      
         return of(null);
      })
    );
    }
    else {
      const putURI = `${environment.task_api_url}/${task.id}`
      return this.httpClient.put(putURI,task).pipe(map(response => {
        this._tasksListSubject.next(<Task[]>response); 
        return of(<Task>response);
      })
      , catchError(err => {      
         return of(null);
      })
    );
    }
  }
  get TaskSubject(): BehaviorSubject<Task> {
    return this._taskSubject;
  }
  get TasksSubject(): BehaviorSubject<Task[]> {
    return this._tasksListSubject;
  }
  setTasksSubject(tasks:Task[]):void {
    this._tasksListSubject.next(tasks);
  }
 
}
