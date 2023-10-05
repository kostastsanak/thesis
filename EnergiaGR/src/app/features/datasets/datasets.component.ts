import { Component, OnInit } from '@angular/core';
import { AppColors } from "src/assets/app-colors";
@Component({
  selector: 'app-datasets',
  templateUrl: './datasets.component.html',
  styleUrls: ['./datasets.component.scss'],
  standalone : true
})
export class DatasetsComponent implements OnInit {
  color = AppColors
  constructor() { }

  ngOnInit() {
  }

}
