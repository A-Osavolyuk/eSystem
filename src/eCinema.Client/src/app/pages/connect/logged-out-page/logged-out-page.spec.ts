import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LoggedOutPage } from './logged-out-page';

describe('LoggedOutPage', () => {
  let component: LoggedOutPage;
  let fixture: ComponentFixture<LoggedOutPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LoggedOutPage]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LoggedOutPage);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
