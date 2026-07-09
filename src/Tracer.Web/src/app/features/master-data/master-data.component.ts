import { Component } from '@angular/core';
import { RouterOutlet, RouterModule } from '@angular/router';
import { MatTabsModule } from '@angular/material/tabs';

@Component({
  selector: 'app-master-data',
  standalone: true,
  imports: [RouterOutlet, RouterModule, MatTabsModule],
  templateUrl: './master-data.component.html',
  styleUrls: ['./master-data.component.scss']
})
export class MasterDataComponent {
  links = [
    { path: 'categories', label: 'Categories' },
    { path: 'locations', label: 'Locations' },
    { path: 'companies', label: 'Companies' },
    { path: 'departments', label: 'Departments' },
    { path: 'manufacturers', label: 'Manufacturers' },
    { path: 'asset-models', label: 'Asset Models' },
    { path: 'suppliers', label: 'Suppliers' },
    { path: 'status-labels', label: 'Status Labels' }
  ];
}
