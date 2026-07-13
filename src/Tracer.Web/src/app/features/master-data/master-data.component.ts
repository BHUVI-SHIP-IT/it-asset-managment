import { Component, computed, inject } from '@angular/core';
import { RouterOutlet, RouterModule } from '@angular/router';
import { MatTabsModule } from '@angular/material/tabs';
import { AuthService } from '../../core/auth/auth.service';
import { Permissions, satisfiesPermission } from '../../core/auth/permissions';

interface MasterDataLink {
  path: string;
  label: string;
  permission: string;
}

const MASTER_DATA_LINKS: MasterDataLink[] = [
  { path: 'categories', label: 'Categories', permission: Permissions.Categories.View },
  { path: 'locations', label: 'Locations', permission: Permissions.Locations.View },
  { path: 'companies', label: 'Companies', permission: Permissions.Settings.View },
  { path: 'departments', label: 'Departments', permission: Permissions.Departments.View },
  { path: 'manufacturers', label: 'Manufacturers', permission: Permissions.Manufacturers.View },
  { path: 'asset-models', label: 'Asset Models', permission: Permissions.AssetModels.View },
  { path: 'suppliers', label: 'Suppliers', permission: Permissions.Suppliers.View },
  { path: 'status-labels', label: 'Status Labels', permission: Permissions.StatusLabels.View },
];

@Component({
  selector: 'app-master-data',
  standalone: true,
  imports: [RouterOutlet, RouterModule, MatTabsModule],
  templateUrl: './master-data.component.html',
  styleUrls: ['./master-data.component.scss']
})
export class MasterDataComponent {
  private auth = inject(AuthService);

  links = computed(() => {
    const perms = this.auth.permissions();
    return MASTER_DATA_LINKS.filter(link => satisfiesPermission(perms, link.permission));
  });
}
