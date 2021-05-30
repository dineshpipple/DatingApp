import { Component, OnInit } from '@angular/core';
import { Message } from '../_model/message';
import { Pagination } from '../_model/pagination';
import { ConfirmService } from '../_services/confirm.service';
import { MessageService } from '../_services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  messages: Message[] = [];
  pagination: Pagination;
  container = 'Unread';
  pageNumber = 1;
  pageSize = 5;
  loading = false;

  constructor(private messageServices: MessageService, private confirmService: ConfirmService ) { }

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages(){
    this.loading= true;
    this.messageServices.getMessages(this.pageNumber, this.pageSize, this.container).subscribe(response => {
      this.messages = response.result;
      this.pagination = response.pagination;
      this.loading = false;
    })
  }

  pageChange(event: any){
    this.pageNumber = event.page;
    this.loadMessages();
  }

  deleteMessage(id:number){
    this.confirmService.confirm('Confirm delete message', 'This can not be undone').subscribe(result =>{
      if(result){
        this.messageServices.deleteMessage(id).subscribe(() => {
          this.messages.splice(this.messages.findIndex(m=> m.id ==id),1)
        })
    
      }
    })
  }


}
