import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Get token directly from localStorage
    const token = localStorage.getItem('auth_token');
    
    console.log(' Interceptor - URL:', req.url);
    console.log(' Interceptor - Token exists:', !!token);
    
    // If token exists, clone request and add Authorization header
    if (token) {
      const cloned = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
      console.log('✅ Token added to request');
      return next.handle(cloned);
    }
    
    // No token, send original request
    console.log('⚠️ No token found');
    return next.handle(req);
  }
}