import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { HttpClientModule } from '@angular/common/http';
import { OAuthModule, OAuthStorage } from 'angular-oauth2-oidc';

import { AppComponent } from './app.component';
import { RouterModule, Routes } from '@angular/router';

import { HTTP_INTERCEPTORS } from '@angular/common/http';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    OAuthModule.forRoot({
      resourceServer: {
          allowedUrls: ['https://localhost:44383/api'],
          sendAccessToken: true, 
      }
    })
  ],
  providers: [
    {
      provide: OAuthStorage, useValue: localStorage
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
  constructor(
  ) {
       
  }

}