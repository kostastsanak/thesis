import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BehaviorSubject } from "rxjs";
import { url } from "src/app/core/services/backend-url";
const BASE_URL = url;

@Injectable({
  providedIn: "root",
})
export class CoreService {
  constructor(private http: HttpClient) {

  }

}
