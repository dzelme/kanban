import * as React from 'react';
import { Link, NavLink } from 'react-router-dom';

export class NavMenu extends React.Component<{}, { version: String }> {

    constructor(props, context) {
        super(props, context)
        this.state = {
            version: ''
        };

        setInterval(() => {
            fetch('api/version')
                .then(res => res.json() as Promise<string>)
                .then(data => {
                    this.setState((prevState) => {
                        if (prevState.version != '' && prevState.version != data)
                            window.location.reload();
                        return { version: data }
                    });
                })
        }, 5000); // todo: make this pretty
    }

    public render() {
        return <div className='main-nav'>
            <div className='navbar navbar-inverse'>
                <div className='navbar-header'>
                    <button type='button' className='navbar-toggle' data-toggle='collapse' data-target='.navbar-collapse'>
                        <span className='sr-only'>Toggle navigation</span>
                        <span className='icon-bar'></span>
                        <span className='icon-bar'></span>
                        <span className='icon-bar'></span>
                        <span className='icon-bar'></span>
                    </button>
                    <Link className='navbar-brand' to={'/'}>ESL.CO.React</Link>
                    <span className='version-aside'>{this.state.version}</span>
                </div>
                <div className='clearfix'></div>
                <div className='navbar-collapse collapse'>
                    <ul className='nav navbar-nav'>
                        <li>
                            <NavLink to={'/'} exact activeClassName='active'>
                                <span className='glyphicon glyphicon-home'></span> Presentation
                            </NavLink>
                        </li>
                        <li>
                            <NavLink to={'/admin/presentations'} activeClassName='active'>
                                <span className='glyphicon glyphicon-th-list'></span> PresentationList
                            </NavLink>
                        </li>
                        <li>
                            <NavLink to={'/statistics'} activeClassName='active'>
                                <span className='glyphicon glyphicon-signal'></span> Statistics
                            </NavLink>
                        </li>
                        <li>
                            <NavLink to={'/login'} activeClassName='active'>
                                <span className='glyphicon glyphicon-signal'></span> Login
                            </NavLink>
                        </li>
                    </ul>
                </div>
            </div>
        </div>;
    }
}