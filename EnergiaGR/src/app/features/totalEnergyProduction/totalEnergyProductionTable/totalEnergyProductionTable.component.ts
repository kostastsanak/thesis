import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { BehaviorSubject, Subscription } from 'rxjs';
import { MaterialFormModule } from 'src/app/shared/material-form.module';
import { MaterialMinModule } from 'src/app/shared/material-min.module';
import { Production, TotalEnergyProductionRequest, TotalEnergyProductionResponse } from '../model/totalEnergyProduction.model';
import { Pagging } from 'src/app/core/models/core.models';
import { TotalEnergyProductionService } from '../service/totalEnergyProduction.service';
@Component({
  selector: 'app-totalEnergyProductionTable',
  templateUrl: './totalEnergyProductionTable.component.html',
  styleUrls: ['./totalEnergyProductionTable.component.scss'],
  imports: [MaterialMinModule, MaterialFormModule],
  standalone: true
})
export class TotalEnergyProductionTableComponent implements OnInit {

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  @Output() correctRequest: EventEmitter<boolean> = new EventEmitter(true);
  myPaginator!: MatPaginator;
  totalEnergyProductionRequest$ = new BehaviorSubject<TotalEnergyProductionRequest | null>(null);
  totalEnergyProductionRequest!: TotalEnergyProductionRequest;
  pagging$ = new BehaviorSubject<Pagging | null>(null);
  pagging!: Pagging;

  production$ = new BehaviorSubject<Production[] | null>(null);
  production!: Production[];
  subscriptions: Subscription = new Subscription();
  dataSource!: MatTableDataSource<any>;
  columnsToDisplay = ["fuel", "energy_mwh", "date","update"];

  constructor(private totalEnergyProductionService: TotalEnergyProductionService) { }

  ngOnInit() {
    this.setRequest();
    this.requestGlobalSearch();
    this.subscriptions.add(
      this.pagging$.subscribe((pagging)=> {
      this.pagging = (pagging as Pagging);
      })
    );
  }


  setRequest() {
    this.subscriptions.add(
      this.totalEnergyProductionService.totalEnergyProductionRequestSubject$.subscribe((data) => {
        if (data) {
          this.totalEnergyProductionRequest = data;
          this.totalEnergyProductionRequest$.next(this.totalEnergyProductionRequest);
        }
      })
    );
  }
  requestGlobalSearch() {
    this.subscriptions.add(
      this.totalEnergyProductionRequest$.subscribe(() => {
        this.subscriptions.add(
          this.totalEnergyProductionService
            .totalEnergyProductionSearch(this.totalEnergyProductionRequest)
            .subscribe((data) => {
              if (data && data.payload.productions) {
                this.correctRequest.emit(true)
                this.production$.next(data.payload.productions);
                this.pagging$.next(data.payload.pagging);
              }else{
                this.correctRequest.emit(false)
              }
            })
        );
      })
    );
  }


  ngAfterViewInit(): void {
    setTimeout(() => {
      this.myPaginator = this.paginator;
      this.pagging != null ? this.pagging.currentPage -1 : 0;
      this.subscriptions.add(
        this.production$.subscribe((data) => {
          if (data != null) {
            this.dataSource = new MatTableDataSource(data);
            this.dataSource.sort = this.sort;
          }
        })
      );
      this.subscriptions.add(
        this.myPaginator.page.subscribe((data) => {
          var pageNumber = data.pageIndex + 1;
          setTimeout(() => {
            //if change page size then go back to first page
            if (
              this.pagging &&
              this.pagging.entriesPerPage != this.myPaginator.pageSize
              ) {
                this.totalEnergyProductionRequest.page = 1;
              this.paginator.pageIndex = 0;
            } else {
              this.totalEnergyProductionRequest.page = pageNumber;
            }
            this.totalEnergyProductionRequest.limit = this.myPaginator.pageSize;
            this.totalEnergyProductionRequest$.next(this.totalEnergyProductionRequest);
          });
        })
      );
    });
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }
  updateForm(){
    this.totalEnergyProductionService.totalEnergyProductionUpdate().subscribe();
  }
  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }
}
