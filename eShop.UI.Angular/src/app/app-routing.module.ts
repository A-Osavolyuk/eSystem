import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppComponent } from './app.component';
import { NotFoundComponent } from './pages/errors/not-found/not-found.component';
import { MainLayoutComponent } from './pages/layouts/main-layout/main-layout.component';
import { LoginComponent } from './pages/account/login/login.component';

const routes: Routes = [
  {
    path: "",
    component: MainLayoutComponent,
    children: [
      {
        path: "products",
        component: AppComponent
      },
      {
        path: "account/login",
        component: LoginComponent,
        title: "Login"
      },
      {
        path: "**",
        component: NotFoundComponent,
        title: "Not Found"
      },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
