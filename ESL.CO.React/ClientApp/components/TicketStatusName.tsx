﻿import * as React from 'react';

export default class TicketStatusName extends React.Component<{ statusName: string }> {
    public render() {

        return <div>
            <h4>{this.props.statusName}</h4>
        </div>

    }
}