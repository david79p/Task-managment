import { BrowserModule } from '@angular/platform-browser';
import { ReactiveFormsModule} from '@angular/forms'
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { TaskComponent } from './components/task/task.component';
import { TasksTableComponent } from './components/tasks-table/tasks-table.component';
import { HomeComponent } from './components/home/home.component';
import { HttpClientModule } from '@angular/common/http';
import { ENumAsStringPipe} from './custom-pipes/enumAsString'
@NgModule({
  declarations: [
    AppComponent,
    TaskComponent,
    TasksTableComponent,
    HomeComponent,
    ENumAsStringPipe
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule,
    HttpClientModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
