import * as React from 'react';

export default class TicketStatusName extends React.Component<{ statusName: string }> {
    public render() {

        return <div className="TicketText">
            <h5>{this.props.statusName}</h5>
        </div>
    }
}