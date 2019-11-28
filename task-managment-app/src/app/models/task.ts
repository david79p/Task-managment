export class Task {
    id:number; 
    name:String;
    todoDate:Date;
    taskPriority:Priority;
    taskStatus:Status;
 
}
export enum Priority{
    High = 1,
    Medium = 2,
    Low = 3
}
export enum Status{
    Open = 1,
    Close = 2
}