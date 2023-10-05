import { Component } from "@angular/core";
import { ThemeServiceService } from "./core/services/themeService.service";
@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.scss","../light-theme.scss","../dark-theme.scss"],
})
export class AppComponent {
  title = "EnergiaGR";
  constructor(public themeService: ThemeServiceService) {}
}
